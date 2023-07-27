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
using TMPro;

namespace Chutpot.FPSParty.Menu
{
    public class PlayMenuView : MenuView
    {
        [SerializeField]
        private Button _joinButton;
        [SerializeField]
        private Button _hostButton;
        [SerializeField]
        private Button _createButton;

        [SerializeField]
        private TMP_InputField _inputField;
        [SerializeField]
        private Toggle _invitationToggle;

        [SerializeField]
        private CanvasGroup _hostCanvasGroup;

        [SerializeField]
        private Button _backButton;

        private string _hostName;
        private bool _isInvitationOnly;

        protected override void Awake()
        {
            base.Awake();
            SelectedGO = _joinButton.gameObject;
            //_joinButton.onClick.AddListener(() => StartCoroutine(OnJoinButtonPressed()));
            //_hostButton.onClick.AddListener(() => StartCoroutine(OnHostButtonPressed()));
            _hostButton.onClick.AddListener(() => OnHostButtonPressed());
            _backButton.onClick.AddListener(() => StartCoroutine(OnBackButtonPressed()));

            _inputField.onValueChanged.AddListener(str => _hostName = str);
            _invitationToggle.onValueChanged.AddListener(toggle => _isInvitationOnly = toggle);
        }

        public override IEnumerator Show()
        {
            yield return base.Show();
            MenuShowSignal.Dispatch(new MenuShowEvent(typeof(PlayMenuView), SelectedGO));

            _hostCanvasGroup.alpha = 0;
            _hostCanvasGroup.blocksRaycasts = false;
            _hostCanvasGroup.interactable = false;
        }

        private IEnumerator OnJoinButtonPressed()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(PlayMenuView), new PlayEventData(PlayEventData.PlayEvent.Join)));
        }

        private void OnHostButtonPressed()
        {
            _hostCanvasGroup.alpha = 1;
            _hostCanvasGroup.blocksRaycasts = true;
            _hostCanvasGroup.interactable = true;
        }

        private IEnumerator OnBackButtonPressed()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(PlayMenuView), new PlayEventData(PlayEventData.PlayEvent.Exit)));

            _hostCanvasGroup.alpha = 0;
            _hostCanvasGroup.blocksRaycasts = false;
            _hostCanvasGroup.interactable = false;
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
