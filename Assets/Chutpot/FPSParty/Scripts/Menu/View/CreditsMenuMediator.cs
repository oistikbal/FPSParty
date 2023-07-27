using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;

namespace Chutpot.FPSParty.Menu
{
    public class CreditsEventData : MenuEventData
    {
        public enum CreditsEvent
        {
            Exit
        }

        public CreditsEvent Event { get; protected set; }

        public CreditsEventData(CreditsEvent CreditsEvent)
        {
            Event = CreditsEvent;
        }
    }

    public class CreditsMenuMediator : MenuMediator
    {
        [Inject]
        public CreditsMenuView View { get; set; }


        public override void OnRegister()
        {
            base.OnRegister();
            View.MenuShowSignal.AddListener(OnMenuShowSignal);
            View.MenuHideSignal.AddListener(OnMenuHideSignal);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            View.MenuShowSignal.RemoveListener(OnMenuShowSignal);
            View.MenuHideSignal.RemoveListener(OnMenuHideSignal);
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
                if (menuEvent.MenuEventData is MainEventData)
                {
                    if (((MainEventData)menuEvent.MenuEventData).Event == MainEventData.MainEvent.Credits)
                    {
                        StartCoroutine(View.Show());
                    }
                }
                else if (menuEvent.MenuEventData is CreditsEventData)
                {
                    StartCoroutine(View.Show());
                }
            }
        }

        protected override void OnMenuMediatorShowSignal(MenuEvent menuEvent)
        {
            base.OnMenuMediatorShowSignal(menuEvent);
        }


    }
}
