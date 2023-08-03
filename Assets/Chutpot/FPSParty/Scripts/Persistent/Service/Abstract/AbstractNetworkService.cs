using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Containers;
using Netcode.Transports.Facepunch;
using Steamworks.Data;
using strange.extensions.context.api;
using strange.extensions.mediation.impl;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Chutpot.FPSParty.Persistent
{
    [Serializable]
    public struct HostCreateData
    {
        public string name;
        public bool isInvitationOnly;

        public HostCreateData(string name, bool isInvitationOnly)
        {
            this.name = name;
            this.isInvitationOnly = isInvitationOnly;
        }

        public override string ToString()
        {
            return "HostName: " + name + " invitation: " + isInvitationOnly.ToString();
        }
    }


    public abstract class AbstractNetworkService
    {
        [Inject(ContextKeys.CONTEXT)]
        public IContext Context { get; set; }
        [Inject]
        public PlayerModel PlayerModel { get; set; }

        protected NetworkServiceView _networkServiceView;

        protected const string _networkServiceAddress = "NetworkService";
        protected const string _lobbyNetworkHandlerAddress = "LobbyNetworkHandler";

        protected GameObject _lobbyHandlerPrefab; //server
        protected LobbyNetworkHandler _lobbyHandler; //server

        [PostConstruct]
        public virtual void Initialize()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(_lobbyNetworkHandlerAddress);
            var op = handle.WaitForCompletion();
            _lobbyHandlerPrefab = op;

            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "HostCreate").OnSignal += OnHostCreateSignal;
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "JoinLobby").OnSignal += signal => JoinLobby(signal);
            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "ExitLobby").OnSignal += signal => StopOrLeave();
            NetworkManager.Singleton.OnClientStarted += OnClientStarted;
            NetworkManager.Singleton.OnClientStopped += OnClientStopped;
        }

        protected virtual void OnClientStarted()
        {
            
        }

        protected virtual void OnClientStopped(bool obj)
        {
        }

        protected virtual void OnClientDisconnected(ulong clientId)
        {
            Debug.Log("OnClientDisconnected");
        }

        protected virtual void OnClientConnected(ulong clientId)
        {
            Debug.Log("OnClientConnected");
        }

        protected virtual void OnServerStarted()
        {
            Debug.Log("OnServerStarted");
        }

        protected virtual void OnHostCreateSignal(Signal signal)
        {
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;


            if (!NetworkManager.Singleton.StartHost())
            {
                var popup = UIPopup.Get("Popup");
                popup.SetTexts("Failed create the server.");
                popup.Show();
                return;
            }

            _lobbyHandler = MonoBehaviour.Instantiate(_lobbyHandlerPrefab).GetComponent<LobbyNetworkHandler>();
        }

        protected virtual void StopOrLeave()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }

            NetworkManager.Singleton.Shutdown();
        }

        protected virtual void JoinLobby(Signal signal) { }

        protected virtual void StartClient()
        {
            if (!NetworkManager.Singleton.StartClient())
            {
                var popup = UIPopup.Get("Popup");
                popup.SetTexts("Failed to join the game");
                popup.Show();
                StopOrLeave();
            }
        }
    }
}
