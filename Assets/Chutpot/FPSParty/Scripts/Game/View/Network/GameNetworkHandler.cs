using Chutpot.FPSParty.Persistent;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Chutpot.FPSParty.Game
{
    public class GameNetworkHandler : NetworkBehaviour
    {
        //Host
        // ClientId, PlayerNetwork pair
        private Dictionary<ulong, NetworkObject> _players;
        private GameObject _playerNetworkPrefab;

        private const string _playerNetworkAddress = "PlayerNetwork";

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsHost)
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(_playerNetworkAddress);
                _playerNetworkPrefab = handle.WaitForCompletion();
                Addressables.Release(handle);

                _players = new Dictionary<ulong, NetworkObject>();
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
            }
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsHost)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
                TerminatePlayers();
            }
        }

        private void OnClientDisconnect(ulong id)
        {
            _players[id].Despawn();
            _players.Remove(id);
        }

        public void InitializePlayers()
        {
            foreach(var id in NetworkManager.ConnectedClientsIds)
            {
                InitializePlayer(id);
            }
        }

        public void InitializePlayer(ulong id)
        {
            _players.Add(id, Instantiate(_playerNetworkPrefab.GetComponent<NetworkObject>()));
            _players[id].SpawnAsPlayerObject(id);
            _players[id].GetComponent<Rigidbody>().position = Vector3.one + Vector3.up * 3f;
        }

        private void TerminatePlayers()
        {
            foreach(var id in NetworkManager.ConnectedClientsIds)
            {
                _players[id].Despawn();
                _players.Remove(id);
            }
        }
    }
}
