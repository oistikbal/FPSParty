using Netcode.Transports.Facepunch;
using Steamworks;
using strange.extensions.mediation.impl;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

namespace Chutpot.FPSParty.Persistent
{
    public class NetworkServiceView : View
    {
        [SerializeField]
        private Button _exitButton;

        private NetworkManager _networkManager;
        private FacepunchTransport _facepunchTransport;
        private UnityTransport _unityTransport;

        public bool IsSteamInitialized { get; protected set; }

        protected override void Awake()
        {
            base.Awake();
            _networkManager = GetComponent<NetworkManager>();
            _exitButton.onClick.AddListener(() => Application.Quit());
            _facepunchTransport = gameObject.AddComponent<FacepunchTransport>();
            if (SteamClient.IsValid)
            {
                IsSteamInitialized = true;
            }
            else 
            {
                IsSteamInitialized = false;
                Destroy(_facepunchTransport);
                _unityTransport = gameObject.AddComponent<UnityTransport>();
                _networkManager.NetworkConfig.NetworkTransport = _unityTransport;
            }

        }
    }
}