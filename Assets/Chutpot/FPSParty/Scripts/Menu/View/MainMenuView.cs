using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using strange.extensions.mediation.impl;
using TMPro;
using Chutpot.FPSParty.Persistent;

namespace Chutpot.FPSParty.Menu
{
    public class MainMenuView : MenuView
    {

        [SerializeField]
        private Button _playButton;
        [SerializeField]
        private Button _optionsButton;
        [SerializeField]
        private Button _creditsButton;
        [SerializeField]
        private Button _exitButton;
        [SerializeField]
        private RawImage _clientImage;
        [SerializeField]
        private TextMeshProUGUI _clientName;

        public PlayerModel PlayerModel { get; set; }
        public MenuExitSignal MenuExitSignal { get; protected set; } 

        protected override void Awake()
        {
            base.Awake();
            SelectedGO = _playButton.gameObject;
            MenuExitSignal = new MenuExitSignal();
            _playButton.onClick.AddListener(() => StartCoroutine(OnPlayButtonPressed()));
            _optionsButton.onClick.AddListener(() => StartCoroutine(OnOptionsButtonPressed()));
            _creditsButton.onClick.AddListener(() => StartCoroutine(OnCreditsButtonPressed()));
            _exitButton.onClick.AddListener(() => StartCoroutine(OnExitButtonPressed()));
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(Show());
            if(!string.IsNullOrEmpty(PlayerModel.Name)) 
            {
                _clientName.text = PlayerModel.Name;
                _clientImage.texture = PlayerModel.ProfileImage.Value.Covert();
            }
        }

        public override IEnumerator Show()
        {
            yield return base.Show();
            MenuShowSignal.Dispatch(new MenuShowEvent(typeof(MainMenuView), SelectedGO));
        }

        public override IEnumerator Hide()
        {
            yield return base.Hide();
        }


        private IEnumerator OnPlayButtonPressed()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(MainMenuView), new MainEventData(MainEventData.MainEvent.Play)));
        }

        private IEnumerator OnOptionsButtonPressed()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(MainMenuView), new MainEventData(MainEventData.MainEvent.Options)));
        }

        private IEnumerator OnCreditsButtonPressed()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(MainMenuView), new MainEventData(MainEventData.MainEvent.Credits)));
        }

        private IEnumerator OnExitButtonPressed()
        {
            yield return Hide();
            MenuExitSignal.Dispatch();
        }
    }
}
