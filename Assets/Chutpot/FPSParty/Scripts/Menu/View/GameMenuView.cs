using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using strange.extensions.mediation.impl;
using UnityEngine.EventSystems;

namespace Chutpot.FPSParty.Menu
{
    public class GameMenuView : MenuView
    {
        [SerializeField]
        private Button _backButton;

        protected override void Awake()
        {
            base.Awake();
            SelectedGO = _backButton.gameObject;
            _backButton.onClick.AddListener(() => StartCoroutine(OnBackButtonPressed()));
        }

        public override IEnumerator Show()
        {
            yield return base.Show();
            MenuShowSignal.Dispatch(new MenuShowEvent(typeof(GameMenuView), SelectedGO));
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
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(GameMenuView), new GameEventData(GameEventData.GameEvent.Exit)));
        }
    }
}
