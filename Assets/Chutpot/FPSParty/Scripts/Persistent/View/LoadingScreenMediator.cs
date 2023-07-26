using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;

namespace Chutpot.Project2D.Persistent
{
    public class LoadingScreenMediator : Mediator
    {
        [Inject]
        public LoadingScreenView View { get; set; }

        [Inject]
        public LoadingScreenSignal LoadingScreenSignal { get; set; }

        public override void OnRegister()
        {
            base.OnRegister();
            View.LoadingScreenSignal.AddListener(OnLoadingScreenChanged);
            //SceneLoadSignal.AddListener(OnSceneLoadChanged);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            View.LoadingScreenSignal.RemoveListener(OnLoadingScreenChanged);
            //SceneLoadSignal.RemoveListener(OnSceneLoadChanged);
        }

        
        public void OnLoadingScreenChanged(LoadingScreenStatus status)
        {
            LoadingScreenSignal.Dispatch(status);
        }
    }
}
