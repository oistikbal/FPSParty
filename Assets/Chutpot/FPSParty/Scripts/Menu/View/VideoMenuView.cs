using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using strange.extensions.mediation.impl;
using Chutpot.FPSParty.Persistent;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

namespace Chutpot.FPSParty.Menu
{
    public class VideoMenuView : MenuView
    {
        public Video Video { get; set; }

        [SerializeField]
        private Slider _resolutionSlider;
        [SerializeField]
        private Slider _fullScreenSlider;
        [SerializeField]
        private Button _vsyncButton;
        [SerializeField]
        private Button _frameCapButton;

        [SerializeField]
        private TextMeshProUGUI _resolutionText;
        [SerializeField]
        private TextMeshProUGUI _fullScreenText;
        [SerializeField]
        private TextMeshProUGUI _vsyncText;
        [SerializeField]
        private TextMeshProUGUI _frameCapText;
        [SerializeField]
        private Button _backButton;
        [SerializeField]
        private Button _applyButton;

        private Resolution[] _resolutions;
        private FullScreenMode _fullscreenMode;
        private Video _video;

        public MenuVideoSignal MenuVideoSignal { get; protected set; }


        protected override void Awake()
        {
            base.Awake();
            _resolutions = Screen.resolutions;
            _resolutionSlider.maxValue = _resolutions.Length - 1;
            MenuVideoSignal = new MenuVideoSignal();

            _resolutionSlider.onValueChanged.AddListener(OnResolutionSliderValueChanged);
            _fullScreenSlider.onValueChanged.AddListener(OnFullScreenSliderValueChanged);
            _vsyncButton.onClick.AddListener(OnVsyncButtonPressed);
            _frameCapButton.onClick.AddListener(OnFrameCapButtonPressed);
            _backButton.onClick.AddListener(() => StartCoroutine(OnBackButtonPressed()));
            _applyButton.onClick.AddListener(OnApplyButtonPressed);
        }

        private void Initialize()
        {
            int index = Array.FindIndex(_resolutions, res => res.width == Screen.width && res.height == Screen.height && res.refreshRate == Screen.currentResolution.refreshRate);

            _video = Video;
            _video.Resolution = Screen.currentResolution;
            _video.ScreenMode = Screen.fullScreenMode;

            _resolutionSlider.value = index;
            _resolutionText.text = _video.Resolution.ToString();
            _fullscreenMode = _video.ScreenMode;
            _fullScreenText.text = Enum.GetName(typeof(FullScreenMode), _fullscreenMode);
            _vsyncText.text = _video.Vsync ? "ON" : "OFF";
            _frameCapText.text = _video.LimitFrameRate ? "ON" : "OFF";
        }

        public override IEnumerator Show()
        {
            Initialize();
            yield return base.Show();
            MenuShowSignal.Dispatch(new MenuShowEvent(typeof(VideoMenuView), SelectedGO));
        }

        protected override void OnCancelPressed(InputAction.CallbackContext obj)
        {
            base.OnCancelPressed(obj);
            if (IsShowing)
            {
                _backButton.onClick.Invoke();
            }
        }
        private void OnResolutionSliderValueChanged(float value)
        {
            _resolutionText.text = _resolutions[Mathf.Clamp((int)value, 0, _resolutions.Length - 1)].ToString();
            _video.Resolution = _resolutions[Mathf.Clamp((int)value, 0, _resolutions.Length - 1)];
        }

        private void OnFullScreenSliderValueChanged(float value)
        {
            _fullscreenMode = (FullScreenMode)value;
            _fullScreenText.text = Enum.GetName(typeof(FullScreenMode), _fullscreenMode);
            _video.ScreenMode = _fullscreenMode;
        }

        private void OnVsyncButtonPressed()
        {
            _video.Vsync = !_video.Vsync;
            _vsyncText.text = _video.Vsync ? "ON" : "OFF";
        }

        private void OnFrameCapButtonPressed()
        {
            _video.LimitFrameRate = !_video.LimitFrameRate;
            _frameCapText.text = _video.LimitFrameRate ? "ON" : "OFF";
        }

        private IEnumerator OnBackButtonPressed()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(VideoMenuView), new VideoEventData(VideoEventData.VideoEvent.Exit)));
        }

        private void OnApplyButtonPressed()
        {
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(VideoMenuView), new VideoEventData(VideoEventData.VideoEvent.Apply)));
            MenuVideoSignal.Dispatch(_video);
        }
    }
}
