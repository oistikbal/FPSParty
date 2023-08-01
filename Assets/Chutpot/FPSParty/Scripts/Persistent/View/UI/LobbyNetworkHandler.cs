using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using Steamworks;
using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
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
            return Id == other.Id && Status == other.Status;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Id);
            serializer.SerializeValue(ref Status);
        }
    }

    public struct FPSLobby : INetworkSerializable
    {
        public FixedString32Bytes LobbyName;
        public FPSMap Map;

        public FPSLobby(FixedString32Bytes lobbyName,FPSMap map)
        {
            this.LobbyName = lobbyName;
            this.Map = map;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref LobbyName);
            serializer.SerializeValue(ref Map);
        }
    }


    public class LobbyNetworkHandler : NetworkBehaviour
    {
        //Network
        private NetworkList<FPSClient> _clients;
        private NetworkVariable<FPSLobby> _lobby;

        //Client
        private SignalStream _updatePlayerStream;
        private SignalStream _disconnectStream;
        private TextMeshProUGUI _lobbyName;
        private TextMeshProUGUI _mapName;
        private UnityEngine.UI.Image _mapImage;
        [SerializeField]
        private Sprite[] _coverImages;

        //Host

        //This is never updated at client;
        public Lobby Lobby;

        public const int MaxPlayer = 8;

        private void Awake()
        {
            _lobbyName = UITag.GetFirstTag("MainMenuUI", "LobbyName").GetComponentInChildren<TextMeshProUGUI>();
            _mapName = UITag.GetFirstTag("MainMenuUI", "MapName").GetComponent<TextMeshProUGUI>();
            _mapImage = UITag.GetFirstTag("MainMenuUI", "MapImage").GetComponent<UnityEngine.UI.Image>();

            _clients = new NetworkList<FPSClient>();
            _lobby = new NetworkVariable<FPSLobby>();

            _updatePlayerStream = Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "UpdatePlayer");
            _disconnectStream = Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "OnDisconnect");

            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "MapChange").OnSignal += signal => UpdateMapServerRpc();
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "ReadyButton").OnSignal += signal => UpdatePlayerStatusServerRpc();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _clients.OnListChanged += changeEvent => UpdatePlayer(changeEvent);
            _lobby.OnValueChanged += OnLobbyUpdated;

            if (IsHost)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
                NetworkManager.Singleton.OnServerStarted += OnServerStarted;

                var fpsLobby = new FPSLobby();
                if (Lobby.Id != 0)
                    fpsLobby.LobbyName = Lobby.GetData("Name");

                fpsLobby.Map = FPSMap.Random;
                _lobby.Value = fpsLobby;

                foreach (var client in NetworkManager.Singleton.ConnectedClientsIds)
                {
                    OnClientConnected(client);
                }
            }


            //Update Lobby and Clients locally
            UpdatePlayers(_clients.GetEnumerator());
            UpdateLobby(_lobby.Value);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsHost)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
                NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            }
            _disconnectStream.SendSignal();
        }

        private void OnServerStarted()
        {
        }

        private void OnClientDisconnect(ulong id)
        {
            if(id != NetworkManager.LocalClientId) 
            {
                if (!_clients.Remove(FindClientIndex(_clients.GetEnumerator(), id)))
                    Debug.LogError("Tried remove a non-existing client!");
            }
        }

        private void OnClientConnected(ulong id)
        {
            if(NetworkManager.Singleton.ConnectedClients.Count >= LobbyNetworkHandler.MaxPlayer)
            {
                NetworkManager.Singleton.DisconnectClient(id);
            }

            ulong steamId = 0;
            if (Lobby.Id.Value != 0)
            {
                steamId = Lobby.Members.ElementAt((int)Lobby.MemberCount -1).Id.Value;
            }

            FPSClient client = new FPSClient(id, FPSClientStatus.Unready, steamId);
            _clients.Add(client);
        }

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
            _updatePlayerStream.SendSignal<NetworkListEvent<FPSClient>>(changeEvent);
        }

        private void UpdatePlayers(IEnumerator<FPSClient> fpsClients)
        {
            _updatePlayerStream.SendSignal<IEnumerator<FPSClient>>(fpsClients);
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
        private void UpdatePlayerStatusServerRpc(ServerRpcParams rpcParams = default)
        {
            var client = FindClientIndex(_clients.GetEnumerator(), rpcParams.Receive.SenderClientId);
            if (client.Status == FPSClientStatus.Unready)
                client.Status = FPSClientStatus.Ready;
            else if (client.Status == FPSClientStatus.Ready)
                client.Status = FPSClientStatus.Unready;

            _clients[_clients.IndexOf(FindClientIndex(_clients.GetEnumerator(), rpcParams.Receive.SenderClientId))] = client;
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = true)]
        private void UpdateMapServerRpc()
        {
            var map = _lobby.Value.Map;
            map = map + 1;
            if (map > Enum.GetValues(typeof(FPSMap)).Cast<FPSMap>().Last())
                map = FPSMap.Random;
            var lobby = _lobby.Value;
            lobby.Map = map;
            _lobby.Value = lobby;
        }

        private FPSClient FindClientIndex(IEnumerator<FPSClient> clients, ulong clientId)
        {
            while (clients.MoveNext())
            {
                if (clients.Current.Id == clientId)
                    return clients.Current;
            }

            return new FPSClient();
        }
    }
}
