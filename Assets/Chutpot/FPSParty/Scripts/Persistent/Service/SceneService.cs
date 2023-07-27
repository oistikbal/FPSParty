using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;
using strange.extensions.context.api;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

namespace Chutpot.FPSParty.Persistent
{
    public class SceneService
    {
        [Inject(ContextKeys.CONTEXT)]
        public IContext Context { get; set; }


        [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject Contextv { get; set; }

        [Inject]
        public LoadingScreenSignal LoadingScreenSignal { get; set; }

        private LoadingScreenView _loadingScreenView;

        private const string _loadingScreenAddress = "LoadingScreen";

        public SceneService()
        {
        }

        [PostConstruct]
        public void Initialize()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(_loadingScreenAddress);
            var op = handle.WaitForCompletion();
            var go = MonoBehaviour.Instantiate(handle.Result);
            Context.AddView(go.GetComponent<View>());
            _loadingScreenView = go.GetComponent<LoadingScreenView>();
        }

        public void LoadScene(string sceneAddress)
        {
            _loadingScreenView.LoadScene(sceneAddress);
        }
    }
}
