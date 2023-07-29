using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Chutpot.FPSParty.Menu
{
    public class HostMenuView : MenuView
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
            MenuShowSignal.Dispatch(new MenuShowEvent(typeof(HostMenuView), SelectedGO));
        }
        private IEnumerator OnBackButtonPressed()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(HostMenuView), new LanguageEventData(LanguageEventData.LanguageEvent.Exit)));
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
