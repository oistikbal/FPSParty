using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;
using Chutpot.FPSParty.Persistent;

namespace Chutpot.FPSParty.Menu
{
    public class MainEventData : MenuEventData
    {
        public enum MainEvent
        {
            Play,
            Options,
            Credits,
            Exit
        }

        public MainEvent Event { get; protected set; }

        public MainEventData(MainEvent playEvent)
        {
            Event = playEvent;
        }
    }

    public class MainMenuMediator : MenuMediator
    {
        [Inject]
        public MainMenuView View { get; set; }
        [Inject]
        public MenuExitSignal MenuMediatorExitSignal { get; set; }
        [Inject]
        public PlayerModel PlayerModel { get; set; }

        public override void OnRegister()
        {
            base.OnRegister();
            View.MenuShowSignal.AddListener(OnMenuShowSignal);
            View.MenuHideSignal.AddListener(OnMenuHideSignal);
            View.MenuPointerSignal.AddListener(OnMenuPointerSignal);
            View.MenuPreHideSignal.AddListener(OnMenuPreHideSignal);
            View.MenuExitSignal.AddListener(OnMenuExitSignal);
            View.PlayerModel = PlayerModel;
        }

        public override void OnRemove()
        {
            base.OnRemove();
            View.MenuShowSignal.RemoveListener(OnMenuShowSignal);
            View.MenuHideSignal.RemoveListener(OnMenuHideSignal);
            View.MenuPointerSignal.RemoveListener(OnMenuPointerSignal);
            View.MenuPreHideSignal.RemoveListener(OnMenuPreHideSignal);
            View.MenuExitSignal.RemoveListener(OnMenuExitSignal);
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
                    if (((PlayEventData)menuEvent.MenuEventData).Event ==PlayEventData.PlayEvent.Exit)
                    {
                        StartCoroutine(View.Show());
                    }
                }
                else if (menuEvent.MenuEventData is OptionsEventData)
                {
                    if (((OptionsEventData)menuEvent.MenuEventData).Event == OptionsEventData.OptionsEvent.Exit)
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

        private void OnMenuPointerSignal(GameObject gameObject)
        {
            MenuMediatorPointerSignal.Dispatch(gameObject);
        }

        private void OnMenuPreHideSignal()
        {
            MenuMediatorPreHideSignal.Dispatch();
        }

        private void OnMenuExitSignal() 
        {
            MenuMediatorExitSignal.Dispatch();
        }
    }
}
