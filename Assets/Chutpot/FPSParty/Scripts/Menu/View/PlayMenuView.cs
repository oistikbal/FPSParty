using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using strange.extensions.mediation.impl;
using UnityEngine.EventSystems;
using UnityEngine.AddressableAssets;
using System;

namespace Chutpot.FPSParty.Menu
{
    public class PlayMenuView : MenuView
    {
        [SerializeField]
        private Button _playButton1;
        [SerializeField]
        private Button _playButton2;
        [SerializeField]
        private Button _playButton3;

        [SerializeField]
        private Button _backButton;

        protected override void Awake()
        {
            base.Awake();
            SelectedGO = _playButton1.gameObject;
            _playButton1.onClick.AddListener(() => StartCoroutine(OnPlayButton1Pressed()));
            _playButton2.onClick.AddListener(() => StartCoroutine(OnPlayButton2Pressed()));
            _playButton2.onClick.AddListener(() => StartCoroutine(OnPlayButton3Pressed()));
            _backButton.onClick.AddListener(() => StartCoroutine(OnBackButtonPressed()));
        }

        public override IEnumerator Show()
        {
            yield return base.Show();
            MenuShowSignal.Dispatch(new MenuShowEvent(typeof(PlayMenuView), SelectedGO));
        }

        private IEnumerator OnPlayButton1Pressed()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(PlayMenuView), new PlayEventData(PlayEventData.PlayEvent.PlayFirst)));
        }

        private IEnumerator OnPlayButton2Pressed()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(PlayMenuView), new PlayEventData(PlayEventData.PlayEvent.PlaySecond)));
        }

        private IEnumerator OnPlayButton3Pressed()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(PlayMenuView), new PlayEventData(PlayEventData.PlayEvent.PlayThirst)));
        }

        private IEnumerator OnBackButtonPressed()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(PlayMenuView), new PlayEventData(PlayEventData.PlayEvent.Exit)));
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
