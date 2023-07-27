using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;

namespace Chutpot.FPSParty.Menu
{
    public class MainMenuCanvasMediator : MenuMediator
    {
        [Inject]
        public MainMenuCanvasView View { get; set; }

        public override void OnRegister()
        {
            base.OnRegister();
            View.MenuShowSignal.AddListener(OnMenuShowSignal);
            View.MenuHideSignal.AddListener(OnMenuHideSignal);
            View.MenuPointerSignal.AddListener(OnMenuPointerSignal);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            View.MenuShowSignal.RemoveListener(OnMenuShowSignal);
            View.MenuHideSignal.RemoveListener(OnMenuHideSignal);
            View.MenuPointerSignal.RemoveListener(OnMenuPointerSignal);
        }

        private void OnMenuShowSignal(MenuEvent menuEvent)
        {
            MenuMediatorShowSignal.Dispatch(menuEvent);
        }

        private void OnMenuHideSignal(MenuEvent menuEvent)
        {
            MenuMediatorHideSignal.Dispatch(menuEvent);
        }

        protected override void OnMenuMediatorHideSignal(MenuEvent menuEvent)
        {
            base.OnMenuMediatorHideSignal(menuEvent);

            if (menuEvent != null)
            {
                if (menuEvent.MenuEventData is PlayEventData)
                {
                }

            }
        }

        protected override void OnMenuMediatorShowSignal(MenuEvent menuEvent)
        {
            base.OnMenuMediatorShowSignal(menuEvent);
        }

        private void OnMenuPointerSignal(GameObject gameObject)
        {
            MenuMediatorPointerSignal.Dispatch(gameObject);
        }
    }
}
