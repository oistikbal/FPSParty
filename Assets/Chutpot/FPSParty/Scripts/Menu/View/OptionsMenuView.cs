using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using strange.extensions.mediation.impl;
using UnityEngine.EventSystems;

namespace Chutpot.FPSParty.Menu
{
    public class OptionsMenuView : MenuView
    {
        [SerializeField]
        private Button _gameButton;
        [SerializeField]
        private Button _audioButton;
        [SerializeField]
        private Button _videoButton;
        [SerializeField]
        private Button _controllerButton;
        [SerializeField]
        private Button _keyboardButton;
        [SerializeField]
        private Button _languageButton;
        [SerializeField]
        private Button _backButton;


        protected override void Awake()
        {
            base.Awake();
            SelectedGO = _gameButton.gameObject;
            _gameButton.onClick.AddListener(() => StartCoroutine(OnGameButtonClick()));
            _audioButton.onClick.AddListener(() => StartCoroutine(OnAudioButtonClick()));
            _videoButton.onClick.AddListener(() => StartCoroutine(OnVideoButtonClick()));
            _controllerButton.onClick.AddListener(() => StartCoroutine(OnControllerButtonClick()));
            _keyboardButton.onClick.AddListener(() => StartCoroutine(OnKeyboardButtonClick()));
            _backButton.onClick.AddListener(() => StartCoroutine(OnBackButtonClick()));
            _languageButton.onClick.AddListener(() => StartCoroutine(OnLanguageButtonPressed()));
        }

        public override IEnumerator Show()
        {
            yield return base.Show();
            MenuShowSignal.Dispatch(new MenuShowEvent(typeof(OptionsMenuView), SelectedGO));
        }

        private IEnumerator OnGameButtonClick()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(OptionsMenuView), new OptionsEventData(OptionsEventData.OptionsEvent.Game)));
        }
        private IEnumerator OnAudioButtonClick()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(OptionsMenuView), new OptionsEventData(OptionsEventData.OptionsEvent.Audio)));
        }
        private IEnumerator OnVideoButtonClick()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(OptionsMenuView), new OptionsEventData(OptionsEventData.OptionsEvent.Video)));
        }

        private IEnumerator OnControllerButtonClick()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(OptionsMenuView), new OptionsEventData(OptionsEventData.OptionsEvent.Controller)));
        }

        private IEnumerator OnKeyboardButtonClick()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(OptionsMenuView), new OptionsEventData(OptionsEventData.OptionsEvent.Keyboard)));
        }

        private IEnumerator OnBackButtonClick()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(OptionsMenuView), new OptionsEventData(OptionsEventData.OptionsEvent.Exit)));
        }

        private IEnumerator OnLanguageButtonPressed()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(OptionsMenuView), new OptionsEventData(OptionsEventData.OptionsEvent.Language)));
        }

        protected override void OnCancelPressed(InputAction.CallbackContext obj)
        {
            base.OnCancelPressed(obj);
            if (IsShowing)
            {
                _backButton.onClick.Invoke();
            }
        }
    }
}
