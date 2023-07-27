using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.mediation.impl;
using Chutpot.FPSParty.Persistent;

namespace Chutpot.FPSParty.Menu
{
    public class VideoEventData : MenuEventData
    {
        public enum VideoEvent
        {
            Apply,
            Exit
        }

        public VideoEvent Event { get; protected set; }

        public VideoEventData(VideoEvent videoEvent)
        {
            Event = videoEvent;
        }
    }


    public class VideoMenuMediator : MenuMediator
    {
        [Inject]
        public VideoMenuView View { get; set; }
        [Inject]
        public MenuVideoSignal MenuMediatorVideoSignal { get; set; }
        [Inject]
        public SettingsSignal SettingsSignal { get; set; }
        [Inject]
        public SettingsModel SettingsModel { get; set; }

        public Video Video;

        public override void OnRegister()
        {
            Video.Vsync = SettingsModel.Settings.VideoData.Vsync;
            Video.LimitFrameRate = SettingsModel.Settings.VideoData.LimitFrameRate;
            base.OnRegister();
            View.MenuShowSignal.AddListener(OnMenuShowSignal);
            View.MenuHideSignal.AddListener(OnMenuHideSignal);
            View.MenuVideoSignal.AddListener(OnMenuVideoSignal);
            View.MenuPointerSignal.AddListener(OnMenuPointerSignal);
            View.MenuPreHideSignal.AddListener(OnMenuPreHideSignal);
            SettingsSignal.AddListener(OnMediatorSettingsSignal);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            View.MenuShowSignal.RemoveListener(OnMenuShowSignal);
            View.MenuHideSignal.RemoveListener(OnMenuHideSignal);
            View.MenuVideoSignal.RemoveListener(OnMenuVideoSignal);
            View.MenuPointerSignal.RemoveListener(OnMenuPointerSignal);
            View.MenuPreHideSignal.RemoveListener(OnMenuPreHideSignal);
            SettingsSignal.RemoveListener(OnMediatorSettingsSignal);
        }

        private void OnMenuShowSignal(MenuEvent menuEvent)
        {
            MenuMediatorShowSignal.Dispatch(menuEvent);
        }

        private void OnMenuHideSignal(MenuEvent menuEvent)
        {
            MenuMediatorHideSignal.Dispatch(menuEvent);
        }

        private void OnMenuVideoSignal(Video video) 
        {
            MenuMediatorVideoSignal.Dispatch(video);
        }

        protected override void OnMenuMediatorHideSignal(MenuEvent menuEvent)
        {
            base.OnMenuMediatorHideSignal(menuEvent);

            if (menuEvent != null)
            {
                if (menuEvent.MenuEventData is OptionsEventData)
                {
                    if (((OptionsEventData)menuEvent.MenuEventData).Event == OptionsEventData.OptionsEvent.Video)
                    {
                        View.Video = Video;
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

        private void OnMediatorSettingsSignal(Settings settings) 
        {
            Video.Vsync = settings.VideoData.Vsync;
            Video.LimitFrameRate = settings.VideoData.LimitFrameRate;
        }
    }
}
