using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;

namespace Chutpot.FPSParty.Menu
{
    public class PlayEventData : MenuEventData
    {
        public enum PlayEvent 
        {
            PlayFirst,
            PlaySecond,
            PlayThirst,
            Exit
        }

        public PlayEvent Event { get; protected set; }

        public PlayEventData(PlayEvent playEvent)
        {
            Event = playEvent;
        }
    }

    public class PlayMenuMediator : MenuMediator
    {
        [Inject]
        public PlayMenuView View { get; set; }
        [Inject]
        public MenuPlaySignal MenuPlaySignal { get; set; }

        public override void OnRegister()
        {
            base.OnRegister();
            View.MenuShowSignal.AddListener(OnMenuShowSignal);
            View.MenuHideSignal.AddListener(OnMenuHideSignal);
            View.MenuPointerSignal.AddListener(OnMenuPointerSignal);
            View.MenuPreHideSignal.AddListener(OnMenuPreHideSignal);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            View.MenuShowSignal.RemoveListener(OnMenuShowSignal);
            View.MenuHideSignal.RemoveListener(OnMenuHideSignal);
            View.MenuPointerSignal.RemoveListener(OnMenuPointerSignal);
            View.MenuPreHideSignal.RemoveListener(OnMenuPreHideSignal);
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

            if(menuEvent != null) 
            {
                if(menuEvent.MenuEventData is MainEventData)
                {
                    if (((MainEventData)menuEvent.MenuEventData).Event == MainEventData.MainEvent.Play)
                    {
                        StartCoroutine(View.Show());
                    }
                }
                if(menuEvent.MenuEventData is PlayEventData) 
                {
                    if (((PlayEventData)menuEvent.MenuEventData).Event == PlayEventData.PlayEvent.PlayFirst)
                    {
                        MenuPlaySignal.Dispatch(new PlayEventData(PlayEventData.PlayEvent.PlayFirst));
                    }
                    else if (((PlayEventData)menuEvent.MenuEventData).Event == PlayEventData.PlayEvent.PlaySecond)
                    {
                        MenuPlaySignal.Dispatch(new PlayEventData(PlayEventData.PlayEvent.PlaySecond));
                    }
                    else if (((PlayEventData)menuEvent.MenuEventData).Event == PlayEventData.PlayEvent.PlayThirst)
                    {
                        MenuPlaySignal.Dispatch(new PlayEventData(PlayEventData.PlayEvent.PlayThirst));
                    }
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

        private void OnMenuPreHideSignal()
        {
            MenuMediatorPreHideSignal.Dispatch();
        }
    }
}
