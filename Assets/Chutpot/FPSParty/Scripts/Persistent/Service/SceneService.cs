using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;
using strange.extensions.context.api;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;
using Steamworks.Data;
using Doozy.Runtime.Signals;

namespace Chutpot.FPSParty.Persistent
{
    public class SceneService
    {
        [Inject(ContextKeys.CONTEXT)]
        public IContext Context { get; set; }


        [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject ContextView { get; set; }

        [Inject]
        public LoadingScreenSignal LoadingScreenSignal { get; set; }

        private const string _baseMapSceneAddress = "Map/";
        private const string _loadingScreenAddress = "LoadingScreen";

        private LoadingScreenView _loadingScreenView;

        [PostConstruct]
        public void Initialize()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(_loadingScreenAddress);
            var op = handle.WaitForCompletion();
            var go = MonoBehaviour.Instantiate(handle.Result);
            Context.AddView(go.GetComponent<View>());
            _loadingScreenView = go.GetComponent<LoadingScreenView>();

            Doozy.Runtime.Signals.SignalsService.GetStream("MainMenuUI", "LoadGame").OnSignal += OnLoadGame;
        }

        private void OnLoadGame(Doozy.Runtime.Signals.Signal signal)
        {
            _loadingScreenView.LoadScene(_baseMapSceneAddress + FPSMap.Dust2.ToString());
        }
    }
}
