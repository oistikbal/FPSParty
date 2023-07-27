using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using strange.extensions.context.api;
using Cinemachine;
using System.Collections.Generic;
using strange.extensions.mediation.impl;

namespace Chutpot.FPSParty.Persistent
{
    public class CameraService
    {
        [Inject(ContextKeys.CONTEXT)]
        public IContext Context { get; set; }

        [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject ContextView { get; set; }

        private const string _cameraPrefabAddress = "Camera";
        private const string _virtualCamerasPrefabAddress = "VirtualCameras";


        [PostConstruct]
        public void Initialize()
        {
            /*
            var handle = Addressables.LoadAssetAsync<GameObject>(_cameraPrefabAddress);
            var op = handle.WaitForCompletion();
            var go = MonoBehaviour.Instantiate(handle.Result);
            _letterBoxer = go.GetComponent<LetterBoxer>();
            Context.AddView(go.GetComponent<View>());

            handle = Addressables.LoadAssetAsync<GameObject>(_virtualCamerasPrefabAddress);
            op = handle.WaitForCompletion();
            _virtualCamerasView = MonoBehaviour.Instantiate(handle.Result).GetComponent<VirtualCamerasView>();
            Context.AddView(_virtualCamerasView.GetComponent<View>());
            */
        }


    }
}
