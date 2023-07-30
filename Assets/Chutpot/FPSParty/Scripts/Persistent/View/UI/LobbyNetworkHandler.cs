using Doozy.Runtime.Signals;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Chutpot.FPSParty.Persistent
{
    public enum FPSClientStatus
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
        public FPSClientStatus Status;

        public FPSClient(ulong id, FPSClientStatus status)
        {
            this.Id = id;
            this.Status = status;
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

        private void Awake()
        {
            _clients = new NetworkList<FPSClient>();
            _clients.OnListChanged += OnClientsChanged;
            _updateLobbyStream = Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "UpdateLobby");


        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

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
            _clients.RemoveAt((int)id);
        }

        private void OnClientConnected(ulong id)
        {
            FPSClient client = new FPSClient(id, FPSClientStatus.Unready);

            _clients.Add(client);
        }

        private void OnClientsChanged(NetworkListEvent<FPSClient> changeEvent)
        {
            _updateLobbyStream.SendSignal<NetworkList<FPSClient>>(_clients);
        }
    }
}
