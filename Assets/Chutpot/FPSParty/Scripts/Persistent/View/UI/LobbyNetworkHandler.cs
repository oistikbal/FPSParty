using Doozy.Runtime.Signals;
using Steamworks;
using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public class LobbyNetworkHandler : NetworkBehaviour
    {

        private NetworkList<FPSClient> _clients;
        SignalStream _updateLobbyStream;
        SignalStream _disconnectStream;

        //This lobby never updated at Client!
        public Lobby Lobby;

        private void Awake()
        {
            _clients = new NetworkList<FPSClient>();
            _updateLobbyStream = Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "UpdateLobby");
            _disconnectStream = Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "OnDisconnect");
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _clients.OnListChanged += OnClientsChanged;
            if (IsHost)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
                NetworkManager.Singleton.OnServerStarted += OnServerStarted;


                foreach(var client in NetworkManager.Singleton.ConnectedClientsIds)
                {
                    OnClientConnected(client);
                }
            }
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
            Debug.Log(id);
            FPSClient client = new FPSClient(id, FPSClientStatus.Off, 0);
            _clients.Insert((int)id, client);
        }

        private void OnClientConnected(ulong id)
        {
            ulong steamId = 0;
            if(Lobby.Id.Value != 0)
            {
                steamId = Lobby.Members.ElementAt((int)id).Id.Value;
            }

            FPSClient client = new FPSClient(id, FPSClientStatus.Unready, steamId);
            _clients.Add(client);
        }

        private void OnClientsChanged(NetworkListEvent<FPSClient> changeEvent)
        {
            _updateLobbyStream.SendSignal<NetworkListEvent<FPSClient>>(changeEvent);
        }

        [ClientRpc]
        public void UpdateClientRpc(ulong id, ClientRpcParams clientRpcParams = default)
        {
            ulong steamId = 0;
            if (Lobby.Id.Value != 0)
            {
                steamId = Lobby.Members.ElementAt((int)id).Id.Value;
            }

            FPSClient client = new FPSClient(id, FPSClientStatus.Unready, steamId);
            _clients.Insert(0, client);
        }
    }
}
