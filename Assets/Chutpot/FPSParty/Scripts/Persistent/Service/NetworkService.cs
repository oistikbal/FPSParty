using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using strange.extensions.context.api;
using strange.extensions.mediation.impl;
using Steamworks.Data;
using Steamworks;
using System.Linq.Expressions;
using Unity.Netcode;
using Netcode.Transports.Facepunch;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Containers;

namespace Chutpot.FPSParty.Persistent
{
    [Serializable]
    public struct HostCreate
    {
        public string name;
        public bool isInvitationOnly;

        public HostCreate(string name, bool isInvitationOnly)
        {
            this.name = name;
            this.isInvitationOnly = isInvitationOnly;
        }

        public override string ToString()
        {
            return "HostName: " + name + " invitation: " + isInvitationOnly.ToString();
        }
    }

    public struct FPSClient 
    {
        public ulong clientId;
        public string name;
    }

    public class FPSLobby
    {
        public string ServerName;
        public FPSClient[] Clients;
        public bool IsInvitationOnly;
        public bool IsHost;

        public FPSLobby(string serverName, bool isInvitationOnly, bool isHost)
        {
            ServerName = serverName;
            IsInvitationOnly = isInvitationOnly;
            IsHost = isHost;
            Clients = new FPSClient[8];

            NetworkManager.Singleton.StartHost();
        }

        ~FPSLobby() 
        { 
        }
    }

    public class NetworkService
    {
        [Inject(ContextKeys.CONTEXT)]
        public IContext Context { get; set; }
        [Inject]
        public PlayerModel PlayerModel { get; set; }

        private FPSLobby _fpsLobby;

        private NetworkServiceView _networkServiceView;
        private FacepunchTransport _facepunchTransport;

        private const string _networkServiceAddress = "NetworkService";

        [PostConstruct]
        public void Initialize()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(_networkServiceAddress);
            var op = handle.WaitForCompletion();

            var go = MonoBehaviour.Instantiate(handle.Result);
            Context.AddView(go.GetComponent<View>());
            _networkServiceView = go.GetComponent<NetworkServiceView>();

            if (_networkServiceView.IsSteamInitialized)
            {
                var playerTask = GetAvatar(SteamClient.SteamId);
                playerTask.Wait();

                PlayerModel.Id = SteamClient.SteamId;
                PlayerModel.ProfileImage = playerTask.Result;
                PlayerModel.Name = SteamClient.Name;
                _networkServiceView.GetComponentInChildren<Canvas>().enabled = false;
                _facepunchTransport = _networkServiceView.GetComponentInChildren<FacepunchTransport>();
            }

            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "HostCreate").OnSignal += OnHostCreateSignal;

            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        ~NetworkService()
        {
        }

        private void OnHostCreateSignal(Signal signal)
        {
            signal.TryGetValue<HostCreate>(out var hostCreate);
        }


        private void OnClientDisconnected(ulong clientId)
        {
        }

        private void OnClientConnected(ulong clientId)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                OnClientConnected(clientId);
            }
        }

        private void OnServerStarted()
        {
        }



        public void StartHost(string hostName, bool isInvitationOnly)
        {
            _fpsLobby = new FPSLobby(hostName, isInvitationOnly, true);
        }

        public void StartClient(ulong targetID)
        {
            if (_facepunchTransport)
            {
                _facepunchTransport.targetSteamId = targetID;
            }

            NetworkManager.Singleton.StartClient();
        }

        public void StopOrLeave()
        {
            NetworkManager.Singleton.Shutdown();
        }


        public async Task<Steamworks.Data.Image?> GetAvatar(SteamId id)
        {
            try
            {
                // Get Avatar using await
                return await SteamFriends.GetLargeAvatarAsync(id);
            }
            catch (Exception e)
            {
                // If something goes wrong, log it
                Debug.Log(e);
                return null;
            }
        }
    }
}