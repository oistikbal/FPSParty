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

namespace Chutpot.FPSParty.Persistent
{
    public class NetworkService
    {
        [Inject(ContextKeys.CONTEXT)]
        public IContext Context { get; set; }
        [Inject]
        public PlayerModel PlayerModel { get; set; }

        private NetworkServiceView _networkServiceView;
        private FacepunchTransport _facepunchTransport;

        private const string _networkServiceAddress = "NetworkService";
        private const string _eventSystemAddress = "EventSystem";

        [PostConstruct]
        public void Initialize()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(_networkServiceAddress);
            var op = handle.WaitForCompletion();

            var go = MonoBehaviour.Instantiate(handle.Result);
            Context.AddView(go.GetComponent<View>());
            _networkServiceView = go.GetComponent<NetworkServiceView>();
#if !UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!_networkServiceView.IsSteamInitialized)
            {
                handle = Addressables.LoadAssetAsync<GameObject>(_eventSystemAddress);
                op = handle.WaitForCompletion();
                go = MonoBehaviour.Instantiate(handle.Result);
                throw new Exception("Throw exception stop further Initializing");
            }
#endif
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
        }

        ~NetworkService() 
        {
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

        public void StartHost(string hostName, bool isInvitationOnly)
        {
            NetworkManager.Singleton.StartHost();
        }

        public void StartClient(ulong targetID)
        {

        }
    }
}