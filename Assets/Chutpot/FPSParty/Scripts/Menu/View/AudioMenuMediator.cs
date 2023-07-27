using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;
using Chutpot.FPSParty.Persistent;

namespace Chutpot.FPSParty.Menu
{
    public class AudioEventData : MenuEventData
    {
        public enum AudioEvent
        {
            Update,
            Exit
        }

        public AudioEvent Event { get; protected set; }

        public AudioEventData(AudioEvent audioEvent)
        {
            Event = audioEvent;
        }
    }

    public class AudioMenuMediator : MenuMediator
    {
        [Inject]
        public AudioMenuView View { get; set; }
        [Inject]
        public MenuAudioSignal MenuMediatorAudioSignal { get; set; }
        [Inject]
        public SettingsSignal SettingsSignal { get; set; }
        [Inject]
        public MenuAudioExitSignal MenuAudioExitSignal { get; set; }
        [Inject]
        public SettingsModel SettingsModel { get; set; }

        private Audio _audio;

        public override void OnRegister()
        {
            _audio = SettingsModel.Settings.Audio;
            base.OnRegister();
            View.MenuShowSignal.AddListener(OnMenuShowSignal);
            View.MenuHideSignal.AddListener(OnMenuHideSignal);
            View.MenuPointerSignal.AddListener(OnMenuPointerSignal);
            View.MenuPreHideSignal.AddListener(OnMenuPreHideSignal);
            View.MenuAudioSignal.AddListener(OnMenuAudioSignal);
            SettingsSignal.AddListener(OnSettingsSignal);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            View.MenuShowSignal.RemoveListener(OnMenuShowSignal);
            View.MenuHideSignal.RemoveListener(OnMenuHideSignal);
            View.MenuPointerSignal.RemoveListener(OnMenuPointerSignal);
            View.MenuPreHideSignal.RemoveListener(OnMenuPreHideSignal);
            View.MenuAudioSignal.RemoveListener(OnMenuAudioSignal);
            SettingsSignal.RemoveListener(OnSettingsSignal);
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
                if (menuEvent.MenuEventData is AudioEventData)
                {
                    if (((AudioEventData)menuEvent.MenuEventData).Event == AudioEventData.AudioEvent.Exit)
                    {
                        _audio = View.Audio;
                        MenuAudioExitSignal.Dispatch(_audio);
                    }
                }
                else if (menuEvent.MenuEventData is OptionsEventData)
                {
                    if (((OptionsEventData)menuEvent.MenuEventData).Event == OptionsEventData.OptionsEvent.Audio)
                    {
                        View.Audio = _audio;
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
            _audio = settings.Audio;
        }
        
        private void OnMenuPointerSignal(GameObject gameObject) 
        {
            MenuMediatorPointerSignal.Dispatch(gameObject);
        }

        private void OnMenuPreHideSignal() 
        {
            MenuMediatorPreHideSignal.Dispatch();
        }

        private void OnMenuAudioSignal(Audio audio) 
        {
            MenuMediatorAudioSignal.Dispatch(audio);
        }
    }
}
