using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using strange.extensions.mediation.impl;
using UnityEngine.EventSystems;
using UnityEngine.AddressableAssets;
using System;
using UnityEditor;

namespace Chutpot.FPSParty.Menu
{
    public class PlayMenuView : MenuView
    {
        [SerializeField]
        private Button _joinButton;
        [SerializeField]
        private Button _hostButton;
        [SerializeField]
        private Button _joinSubButton;
        [SerializeField]
        private Button _hostSubButton;

        [SerializeField]
        private Button _backButton;

        protected override void Awake()
        {
            base.Awake();
            SelectedGO = _joinSubButton.gameObject;
            //_joinButton.onClick.AddListener(() => StartCoroutine(OnJoinButtonPressed()));
            //_hostButton.onClick.AddListener(() => StartCoroutine(OnHostButtonPressed()));
            _joinSubButton.onClick.AddListener(() => OnJoinSubButtonPressed());
            _hostSubButton.onClick.AddListener(() => OnHostSubButtonPressed());
            _backButton.onClick.AddListener(() => StartCoroutine(OnBackButtonPressed()));
        }

        public override IEnumerator Show()
        {
            yield return base.Show();
            MenuShowSignal.Dispatch(new MenuShowEvent(typeof(PlayMenuView), SelectedGO));
        }

        private IEnumerator OnJoinButtonPressed()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(PlayMenuView), new PlayEventData(PlayEventData.PlayEvent.Join)));
        }

        private IEnumerator OnHostButtonPressed()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(PlayMenuView), new PlayEventData(PlayEventData.PlayEvent.Host)));
        }

        private IEnumerator OnBackButtonPressed()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(PlayMenuView), new PlayEventData(PlayEventData.PlayEvent.Exit)));
        }

        private void OnJoinSubButtonPressed()
        {

        }

        private void OnHostSubButtonPressed()
        {

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
