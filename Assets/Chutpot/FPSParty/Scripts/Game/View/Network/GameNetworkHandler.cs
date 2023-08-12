using Chutpot.FPSParty.Persistent;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Chutpot.FPSParty.Game
{
    public enum FPSTeam : byte
    {
        None,
        Red,
        Blue
    }

    public enum FPSWeapon : byte
    {
        Pistol
    }

    public struct FPSGamePlayer : INetworkSerializable, IEquatable<FPSGamePlayer>
    {
        public byte Health;
        public byte Armor;
        public byte IsAlive;
        public byte Kill;
        public FPSTeam Team;
        public FPSWeapon Weapon;

        public bool Equals(FPSGamePlayer other)
        {
            return Health == other.Health && Armor == other.Armor && IsAlive == other.IsAlive && Kill == other.Kill && Team == other.Team && Weapon == other.Weapon;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Health);
            serializer.SerializeValue(ref Armor);
            serializer.SerializeValue(ref IsAlive);
            serializer.SerializeValue(ref Kill);
            serializer.SerializeValue(ref Team);
            serializer.SerializeValue(ref Weapon);
        }
    }

    public class GameNetworkHandler : NetworkBehaviour
    {

        //Network
        private NetworkList<FPSGamePlayer> _fpsPlayers;

        //Host
        // ClientId, PlayerNetwork pair
        private Dictionary<ulong, NetworkObject> _players;

        [SerializeField]
        private GameObject _playerNetworkPrefab;


        private const string _playerNetworkAddress = "PlayerNetwork";

        private void Awake()
        {
            _fpsPlayers = new NetworkList<FPSGamePlayer>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsHost)
            {
                _players = new Dictionary<ulong, NetworkObject>();
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            }
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsHost && !NetworkManager.Singleton.ShutdownInProgress)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                TerminatePlayers();
            }
        }

        private void OnClientDisconnect(ulong id)
        {
            TerminatePlayer(id);
        }

        private void OnClientConnected(ulong id)
        {
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
            _players.Add(id, Instantiate(_playerNetworkPrefab).GetComponent<NetworkObject>());
            _players[id].SpawnWithOwnership(id);
        }


        private void TerminatePlayers()
        {
            foreach(var id in NetworkManager.ConnectedClientsIds)
            {
                TerminatePlayer(id);
            }
        }

        private void TerminatePlayer(ulong id)
        {
            _players[id].Despawn();
            _players.Remove(id);
        }
    }
}
