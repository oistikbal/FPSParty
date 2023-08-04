using Chutpot.FPSParty.Game;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace Chutpot.FPSParty.Persistent
{
    public enum FPSClientStatus : byte
    {
        Off,
        Unready,
        Ready,
        Loading,
        InGame
    }

    public enum FPSMap : byte
    {
        Random,
        Dust2
    }

    public enum FPSLobbyStatus : byte
    {
        Lobby,
        Loading,
        Started
    }

    public struct FPSClient : INetworkSerializable, IEquatable<FPSClient>
    {
        public ulong Id;
        public ulong SteamId;
        public FPSClientStatus Status;

        public FPSClient(ulong id, FPSClientStatus status, ulong steamId)
        {
            this.Id = id;
            this.Status = status;
            this.SteamId = steamId;
        }

        public bool Equals(FPSClient other)
        {
            return Id == other.Id && Status == other.Status && SteamId == other.SteamId;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Id);
            serializer.SerializeValue(ref SteamId);
            serializer.SerializeValue(ref Status);
        }
    }

    public struct FPSLobby : INetworkSerializable
    {
        public FixedString32Bytes LobbyName;
        public FPSMap Map;
        public FPSLobbyStatus LobbyStatus;

        public FPSLobby(FixedString32Bytes lobbyName,FPSMap map, FPSLobbyStatus gameStatus)
        {
            this.LobbyName = lobbyName;
            this.Map = map;
            this.LobbyStatus = gameStatus;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref LobbyName);
            serializer.SerializeValue(ref Map);
            serializer.SerializeValue(ref LobbyStatus);
        }
    }


    public class LobbyNetworkHandler : NetworkBehaviour
    {
        //Network
        private NetworkList<FPSClient> _clients;
        private NetworkVariable<FPSLobby> _lobby;

        //Client
        [SerializeField]
        private Sprite[] _coverImages;

        private TextMeshProUGUI _lobbyName;
        private TextMeshProUGUI _mapName;
        private UnityEngine.UI.Image _mapImage;

        private SignalStream _updatePlayerStream;
        private SignalStream _mapChangeStream;
        private SignalStream _startLobbyStream;
        private SignalStream _readyButtonStream;
        private SignalStream _loadGameStream;
        private SignalStream _gameLoadedStream;
        private SignalStream _startGameStream;

        private bool _isLobbyInitialized;
        private bool _isGameStarted;

        //Host, these values never gets updated at client
        private Coroutine _timeoutCoroutine;
        private GameNetworkHandler _gameNetworkHandler;
        private GameObject _gameNetworkHandlerPrefab;
        private NetworkObject _gameNetworObject;

        private const string _gameNetworkHandlerAddress = "GameNetworkHandler";

        [HideInInspector]
        public int _loadingPlayersCount;
        [HideInInspector]
        public Lobby SteamLobby;

        public const int TimeouTime = 30;
        public const int MaxPlayer = 8;

        private void Awake()
        {
            _isLobbyInitialized = false;
            _lobbyName = UITag.GetFirstTag("MainMenuUI", "LobbyName").GetComponentInChildren<TextMeshProUGUI>();
            _mapName = UITag.GetFirstTag("MainMenuUI", "MapName").GetComponent<TextMeshProUGUI>();
            _mapImage = UITag.GetFirstTag("MainMenuUI", "MapImage").GetComponent<UnityEngine.UI.Image>();

            _clients = new NetworkList<FPSClient>();
            _lobby = new NetworkVariable<FPSLobby>();

            _updatePlayerStream = Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "UpdatePlayer");

            _mapChangeStream = Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "MapChange");
            _startLobbyStream = Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "StartLobby");
            _readyButtonStream = Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "ReadyButton");

            _loadGameStream = Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "LoadGame");
            _gameLoadedStream = Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "GameLoaded");
            _startGameStream = Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "StartGame");

            var handle = Addressables.LoadAssetAsync<GameObject>(_gameNetworkHandlerAddress);
            _gameNetworkHandlerPrefab = handle.WaitForCompletion();
            Addressables.Release(handle);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _mapChangeStream.OnSignal += signal => { if (IsHost) UpdateMapServerRpc(); };
            _startLobbyStream.OnSignal += signal => { if (IsHost) StartGameServerRpc(); };
            _readyButtonStream.OnSignal += signal => UpdatePlayerStatusServerRpc();
            _gameLoadedStream.OnSignal += signal => UpdateLoadingStatusServerRpc();

            _clients.OnListChanged += changeEvent => { UpdatePlayer(changeEvent); };
            _lobby.OnValueChanged += OnLobbyUpdated;

            if (IsHost)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
                NetworkManager.Singleton.OnServerStarted += OnServerStarted;

                var fpsLobby = new FPSLobby();
                if (SteamLobby.Id != 0)
                    fpsLobby.LobbyName = SteamLobby.GetData("Name");

                fpsLobby.LobbyStatus = FPSLobbyStatus.Lobby;
                fpsLobby.Map = FPSMap.Random;
                _lobby.Value = fpsLobby;

                foreach (var client in NetworkManager.Singleton.ConnectedClientsIds)
                {
                    OnClientConnected(client);
                }
            }
        }


        //Host
        public override void OnNetworkDespawn()
        {
            _mapChangeStream.Close();
            _startLobbyStream.Close();
            _readyButtonStream.Close();
            _gameLoadedStream.Close();

            base.OnNetworkDespawn();
            if (IsHost)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
                NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            }
        }

        private void OnServerStarted()
        {
        }

        private void OnClientDisconnect(ulong id)
        {
            try
            {
                if (id != NetworkManager.LocalClientId)
                {
                    if (!_clients.Remove(FindClientWithIndex(_clients.GetEnumerator(), id)))
                        Debug.LogError("Tried remove a non-existing client!");
                }
            }
            //Don't remove when server is going to shutdown, block nullref ex
            catch{ }
        }

        private void OnClientConnected(ulong id)
        {
            if(NetworkManager.Singleton.ConnectedClients.Count >= LobbyNetworkHandler.MaxPlayer)
            {
                NetworkManager.Singleton.DisconnectClient(id);
            }

            ulong steamId = 0;
            if (SteamLobby.Id.Value != 0)
            {
                steamId = SteamLobby.Members.ElementAt((int)SteamLobby.MemberCount -1).Id.Value;
            }

            FPSClient client = new FPSClient(id, FPSClientStatus.Unready, steamId);
            _clients.Add(client);
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
        private void UpdatePlayerStatusServerRpc(ServerRpcParams rpcParams = default)
        {
            if (_lobby.Value.LobbyStatus != FPSLobbyStatus.Lobby)
                return;

            var client = FindClientWithIndex(_clients.GetEnumerator(), rpcParams.Receive.SenderClientId);
            if (client.Status == FPSClientStatus.Unready)
                client.Status = FPSClientStatus.Ready;
            else if (client.Status == FPSClientStatus.Ready)
                client.Status = FPSClientStatus.Unready;

            _clients[_clients.IndexOf(FindClientWithIndex(_clients.GetEnumerator(), rpcParams.Receive.SenderClientId))] = client;
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = true)]
        private void UpdateMapServerRpc()
        {
            if (_lobby.Value.LobbyStatus != FPSLobbyStatus.Lobby)
                return;


            var map = _lobby.Value.Map;
            map = map + 1;
            if (map > Enum.GetValues(typeof(FPSMap)).Cast<FPSMap>().Last())
                map = FPSMap.Random;
            var lobby = _lobby.Value;
            lobby.Map = map;
            _lobby.Value = lobby;
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = true)]
        private void StartGameServerRpc()
        {
            if (_lobby.Value.LobbyStatus != FPSLobbyStatus.Lobby)
                return;

            int readyPlayer = 0;
            foreach(var fpclient in _clients) 
            { 
                if(fpclient.Status == FPSClientStatus.Ready)
                    readyPlayer++;
            }

            //Start game
            if(readyPlayer >= _clients.Count)
            {
                var lobby = _lobby.Value;
                lobby.LobbyStatus = FPSLobbyStatus.Loading;
                _lobby.Value = lobby;

                _loadingPlayersCount = readyPlayer;
                _timeoutCoroutine = StartCoroutine(Timeout());
                LoadGameClientRpc();
            }
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void LoadGameClientRpc()
        {
            _isGameStarted = false;
            _loadGameStream.SendSignal<FPSMap>(_lobby.Value.Map);
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
        public void UpdateLoadingStatusServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (_lobby.Value.LobbyStatus != FPSLobbyStatus.Loading)
                return;

            _loadingPlayersCount--;
            if (_loadingPlayersCount == 0 || _timeoutCoroutine == null)
            {
                if(_timeoutCoroutine != null)
                    StopCoroutine(Timeout());

                _timeoutCoroutine = null;
                var lobby = _lobby.Value;
                lobby.LobbyStatus = FPSLobbyStatus.Started;
                _gameNetworkHandler = Instantiate(_gameNetworkHandlerPrefab).GetComponent<GameNetworkHandler>();
                _gameNetworObject = _gameNetworkHandler.GetComponent<NetworkObject>();
                _gameNetworObject.Spawn();
                StartGameClientRpc();
            }
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        public void StartGameClientRpc()
        {
            if (!_isGameStarted)
            {
                _isGameStarted = true;
                _startGameStream.SendSignal();
            }
        }


        private FPSClient FindClientWithIndex(IEnumerator<FPSClient> clients, ulong targetClientId)
        {
            foreach (var client in _clients) 
            { 
                if (client.Id == targetClientId)
                    return client;
            }

            return new FPSClient();
        }

        //Client
        private void OnLobbyUpdated(FPSLobby oldLobby, FPSLobby newLobby)
        {
            UpdateLobby(newLobby);
        }

        private void UpdateLobby(FPSLobby newLobby)
        {
            _lobbyName.text = newLobby.LobbyName.ToString();
            _mapName.text = newLobby.Map.ToString();
            _mapImage.sprite = _coverImages[(int)newLobby.Map];
        }

        private void UpdatePlayer(NetworkListEvent<FPSClient> changeEvent)
        {
            if (!_isLobbyInitialized)
            {
                _isLobbyInitialized = true;
                UpdatePlayers();
                UpdateLobby(_lobby.Value);
                return;
            }

            _updatePlayerStream.SendSignal<NetworkListEvent<FPSClient>>(changeEvent);
        }

        private void UpdatePlayers()
        {
            _updatePlayerStream.SendSignal<NetworkList<FPSClient>>(_clients);
        }

        private IEnumerator Timeout()
        {
            yield return new WaitForSecondsRealtime(TimeouTime);
            UpdateLoadingStatusServerRpc();
        }
    }
}
