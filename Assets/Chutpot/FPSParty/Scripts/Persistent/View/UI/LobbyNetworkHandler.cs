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
        Default,
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

        //Host

        //This is never updated at client;
        public Lobby Lobby;

        public const int MaxPlayer = 8;

        private void Awake()
        {
            _lobbyName = UITag.GetFirstTag("MainMenuUI", "LobbyName").GetComponentInChildren<TextMeshProUGUI>();
            _mapName = UITag.GetFirstTag("MainMenuUI", "Map").GetComponentInChildren<TextMeshProUGUI>();
            _mapImage = UITag.GetFirstTag("MainMenuUI", "Map").GetComponentInChildren<UnityEngine.UI.Image>();

            _clients = new NetworkList<FPSClient>();
            _lobby = new NetworkVariable<FPSLobby>();

            _lobby.OnValueChanged += OnLobbyUpdated;
            _clients.OnListChanged += changeEvent => UpdatePlayers(_clients.GetEnumerator());

            _updatePlayerStream = Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "UpdatePlayers");
            _disconnectStream = Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "OnDisconnect");
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsHost)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
                NetworkManager.Singleton.OnServerStarted += OnServerStarted;

                var fpsLobby = new FPSLobby();
                if (Lobby.Id != 0)
                    fpsLobby.LobbyName = Lobby.GetData("Name");

                fpsLobby.Map = FPSMap.Default;
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

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void OnServerStarted()
        {
        }

        private void OnClientDisconnect(ulong id)
        {
            if(id != NetworkManager.LocalClientId) 
            {
                _clients.RemoveAt(_clients.IndexOf(FindClientIndex(_clients.GetEnumerator(), id)));
                SendDisconnectedClientRpc(id);
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
            _clients.Insert((int)id, client);
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        public void SendDisconnectedClientRpc(ulong id)
        {
            UpdateDisconnectedPlayer(id);
        }

        private void OnLobbyUpdated(FPSLobby oldLobby, FPSLobby newLobby)
        {
            UpdateLobby(newLobby);
        }

        private void UpdateLobby(FPSLobby newLobby)
        {
            _lobbyName.text = newLobby.LobbyName.ToString();
            _mapName.text = newLobby.Map.ToString();
        }

        private void UpdateDisconnectedPlayer(ulong id)
        {
            _updatePlayerStream.SendSignal<ulong>(id);
        }

        private void UpdatePlayers(IEnumerator<FPSClient> _players)
        {
            _updatePlayerStream.SendSignal<IEnumerator<FPSClient>>(_players);
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
