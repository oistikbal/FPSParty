using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using strange.extensions.mediation.impl;
using Chutpot.FPSParty.Persistent;
using System.Collections;

namespace Chutpot.FPSParty.Menu
{
    public class AudioMenuView : MenuView
    {
        public Audio Audio { get; set; }

        [SerializeField]
        private Slider _masterSlider;
        [SerializeField]
        private Slider _gameSlider;
        [SerializeField]
        private Slider _musicSlider;

        [SerializeField]
        private Button _backButton;

        public MenuAudioSignal MenuAudioSignal { get; protected set; }

        private Audio _audio;

        protected override void Awake()
        {
            base.Awake();
            MenuAudioSignal = new MenuAudioSignal();
            SelectedGO = _masterSlider.gameObject;
            _masterSlider.onValueChanged.AddListener(OnMasterValueChanged);
            _gameSlider.onValueChanged.AddListener(OnGameValueChanged);
            _musicSlider.onValueChanged.AddListener(OnMusicValueChanged);
            _backButton.onClick.AddListener(() => StartCoroutine(OnBackButtonPressed()));
        }

        protected override void Start()
        {
            base.Start();
        }

        public override IEnumerator Show()
        {
            Initialize();
            yield return base.Show();
            MenuShowSignal.Dispatch(new MenuShowEvent(typeof(AudioMenuView), SelectedGO));
        }

        private void Initialize() 
        {
            _audio = Audio;
            _masterSlider.value = _audio.MasterAudio;
            _gameSlider.value = _audio.GameAudio;
            _musicSlider.value = _audio.MusicAudio;
        }

        private void OnMasterValueChanged(float value)
        {
            _audio.MasterAudio = value;
            MenuAudioSignal.Dispatch(_audio);
        }

        private void OnGameValueChanged(float value)
        {
            _audio.GameAudio = value;
            MenuAudioSignal.Dispatch(_audio);
        }

        private void OnMusicValueChanged(float value)
        {
            _audio.MusicAudio = value;
            MenuAudioSignal.Dispatch(_audio);
        }

        protected override void OnCancelPressed(InputAction.CallbackContext obj)
        {
            base.OnCancelPressed(obj);
            if (IsShowing) 
            {
                _backButton.onClick.Invoke();
            }
        }

        private IEnumerator OnBackButtonPressed()
        {
            Audio = _audio;
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(AudioMenuView), new AudioEventData(AudioEventData.AudioEvent.Exit)));
        }
    }
}
