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

        public SceneService()
        {
        }

        [PostConstruct]
        public void Initialize()
        {
            //_loadingScreenView = go.GetComponent<LoadingScreenView>();
        }

        public void LoadScene(string sceneAddress)
        {
        }
    }
}
