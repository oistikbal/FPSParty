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

namespace Chutpot.FPSParty.Persistent
{
    public class NetworkService
    {
        [Inject(ContextKeys.CONTEXT)]
        public IContext Context { get; set; }
        [Inject]
        public PlayerModel PlayerModel { get; set; }

        private NetworkServiceView _networkServiceView;

        private const string _networkServiceAddress = "NetworkService";

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
                throw new Exception("Throw exception stop further Initializing"); ;
#endif
            if (_networkServiceView.IsSteamInitialized)
            {
                var playerTask = GetAvatar(SteamClient.SteamId);
                playerTask.Wait();

                PlayerModel.Id = SteamClient.SteamId;
                PlayerModel.ProfileImage = playerTask.Result;
                PlayerModel.Name = SteamClient.Name;
                go.GetComponent<Canvas>().enabled = false;
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
    }
}