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
        private NetworkManager _networkManager;
        private FacepunchTransport _facepunchTransport;
        private UnityTransport _unityTransport;

        public Action Initialized;

        protected override void Awake()
        {
            base.Awake();
            _networkManager = GetComponent<NetworkManager>();
            _facepunchTransport = gameObject.AddComponent<FacepunchTransport>();
            if (SteamClient.IsValid)
            {
                _networkManager.NetworkConfig.NetworkTransport = _facepunchTransport;
            }
            else 
            {
                Destroy(_facepunchTransport);
                _unityTransport = gameObject.AddComponent<UnityTransport>();
                _networkManager.NetworkConfig.NetworkTransport = _unityTransport;
            }

            Initialized?.Invoke();

        }
    }
}