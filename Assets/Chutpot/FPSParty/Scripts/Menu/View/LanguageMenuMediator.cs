using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;
using Chutpot.FPSParty.Persistent;


namespace Chutpot.FPSParty.Menu
{
    public class LanguageEventData : MenuEventData
    {
        public enum LanguageEvent
        {
            Update,
            Exit
        }

        public LanguageEvent Event { get; protected set; }

        public LanguageEventData(LanguageEvent languageEvent)
        {
            Event = languageEvent;
        }
    }

    public class LanguageMenuMediator : MenuMediator
    {
        [Inject]
        public LanguageMenuView View { get; set; }
        [Inject]
        public SettingsSignal SettingsSignal { get; set; }

        private int _languageIndex;

        public override void OnRegister()
        {
            base.OnRegister();
            View.MenuShowSignal.AddListener(OnMenuShowSignal);
            View.MenuHideSignal.AddListener(OnMenuHideSignal);
            SettingsSignal.AddListener(OnSettingsSignal);
            View.MenuPointerSignal.AddListener(OnMenuPointerSignal);
            View.MenuPreHideSignal.AddListener(OnMenuPreHideSignal);
            View.MenuLanguageSignal.AddListener(OnMenuLanguageSignal);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            View.MenuShowSignal.RemoveListener(OnMenuShowSignal);
            View.MenuHideSignal.RemoveListener(OnMenuHideSignal);
            SettingsSignal.RemoveListener(OnSettingsSignal);
            View.MenuPointerSignal.RemoveListener(OnMenuPointerSignal);
            View.MenuPreHideSignal.RemoveListener(OnMenuPreHideSignal);
            View.MenuLanguageSignal.RemoveListener(OnMenuLanguageSignal);
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
                if (menuEvent.MenuEventData is OptionsEventData)
                {
                    if (((OptionsEventData)menuEvent.MenuEventData).Event == OptionsEventData.OptionsEvent.Language)
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

        private void OnSettingsSignal(Settings settings)
        {
            _languageIndex = settings.LanguageIndex;
        }

        private void OnMenuPointerSignal(GameObject gameObject)
        {
            MenuMediatorPointerSignal.Dispatch(gameObject);
        }

        private void OnMenuPreHideSignal()
        {
            MenuMediatorPreHideSignal.Dispatch();
        }

        private void OnMenuLanguageSignal(int languageIndex) 
        {
            _languageIndex = languageIndex;
        }
    }
}
