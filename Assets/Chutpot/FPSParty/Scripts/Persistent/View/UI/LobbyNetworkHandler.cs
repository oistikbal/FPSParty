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

        private const int MaxPlayer = 8;

        private void Awake()
        {
            _clients = new NetworkList<FPSClient>();
            _updateLobbyStream = Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "UpdateLobby");
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


                foreach (var client in NetworkManager.Singleton.ConnectedClientsIds)
                {
                    OnClientConnected(client);
                }
            }

            GetClientsServerRpc();
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
            _clients.RemoveAt(_clients.IndexOf(FindClientIndex(_clients.GetEnumerator(), id)));
            SendDisconnectedClientRpc(id);
        }

        private void OnClientConnected(ulong id)
        {
            if(NetworkManager.Singleton.ConnectedClients.Count >= 8)
            {
                NetworkManager.Singleton.DisconnectClient(id);
            }

            ulong steamId = 0;
            if (Lobby.Id.Value != 0)
            {
                steamId = Lobby.Members.ElementAt((int)id).Id.Value;
            }

            FPSClient client = new FPSClient(id, FPSClientStatus.Unready, steamId);
            _clients.Insert((int)id, client);
            SendClientsClientRpc();
        }

        [ServerRpc(Delivery = RpcDelivery.Reliable, RequireOwnership = false)]
        private void GetClientsServerRpc()
        {
            SendClientsClientRpc();
        }


        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void SendClientsClientRpc()
        {
            _updateLobbyStream.SendSignal<IEnumerator<FPSClient>>(_clients.GetEnumerator());
        }

        [ClientRpc(Delivery = RpcDelivery.Reliable)]
        private void SendDisconnectedClientRpc(ulong id)
        {
            _updateLobbyStream.SendSignal<ulong>(id);
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
