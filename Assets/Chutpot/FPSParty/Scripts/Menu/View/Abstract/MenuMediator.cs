using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;

namespace Chutpot.FPSParty.Menu
{
    public abstract class MenuMediator : Mediator
    {
        [Inject]
        public MenuShowSignal MenuMediatorShowSignal { get; set; }
        [Inject]
        public MenuHideSignal MenuMediatorHideSignal { get; set; }
        [Inject]
        public MenuPointerSignal MenuMediatorPointerSignal { get; set; }
        [Inject]
        public MenuPreHideSignal MenuMediatorPreHideSignal { get; set; }

        public override void OnRegister()
        {
            base.OnRegister();
            MenuMediatorShowSignal.AddListener(OnMenuMediatorShowSignal);
            MenuMediatorHideSignal.AddListener(OnMenuMediatorHideSignal);
            MenuMediatorPointerSignal.AddListener(OnMenuMediatorPointerSignal);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            MenuMediatorShowSignal.RemoveListener(OnMenuMediatorShowSignal);
            MenuMediatorHideSignal.RemoveListener(OnMenuMediatorHideSignal);
            MenuMediatorPointerSignal.RemoveListener(OnMenuMediatorPointerSignal);
        }

        protected virtual void OnMenuMediatorShowSignal(MenuEvent menuEvent)
        {
        }

        protected virtual void OnMenuMediatorHideSignal(MenuEvent menuEvent) 
        {
        }

        protected virtual void OnMenuMediatorPointerSignal(GameObject gameObject) 
        {
        }

    }
}
