using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;

namespace Chutpot.FPSParty.Menu
{
    public class OptionsEventData : MenuEventData
    {
        public enum OptionsEvent
        {
            Game,
            Audio,
            Video,
            Controller,
            Keyboard,
            Language,
            Exit
        }

        public OptionsEvent Event { get; protected set; }

        public OptionsEventData(OptionsEvent optionsEvent)
        {
            Event = optionsEvent;
        }
    }

    public class OptionsMenuMediator : MenuMediator
    {
        [Inject]
        public OptionsMenuView View { get; set; }

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

            if (menuEvent != null)
            {
                if (menuEvent.MenuEventData is MainEventData)
                {
                    if (((MainEventData)menuEvent.MenuEventData).Event == MainEventData.MainEvent.Options)
                    {
                        StartCoroutine(View.Show());
                    }
                }
                else if(menuEvent.MenuEventData is VideoEventData) 
                {
                    if (((VideoEventData)menuEvent.MenuEventData).Event == VideoEventData.VideoEvent.Exit)
                    {
                        StartCoroutine(View.Show());
                    }
                }
                else if (menuEvent.MenuEventData is AudioEventData)
                {
                    if (((AudioEventData)menuEvent.MenuEventData).Event == AudioEventData.AudioEvent.Exit)
                    {
                        StartCoroutine(View.Show());
                    }
                }
                else if (menuEvent.MenuEventData is GameEventData)
                {
                    if (((GameEventData)menuEvent.MenuEventData).Event == GameEventData.GameEvent.Exit)
                    {
                        StartCoroutine(View.Show());
                    }
                }
                else if (menuEvent.MenuEventData is LanguageEventData)
                {
                    if (((LanguageEventData)menuEvent.MenuEventData).Event == LanguageEventData.LanguageEvent.Exit)
                    {
                        StartCoroutine(View.Show());
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
