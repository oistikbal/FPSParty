using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using strange.extensions.mediation.impl;
using UnityEngine.EventSystems;

namespace Chutpot.FPSParty.Menu
{
    public class CreditsMenuView : MenuView
    {
        [SerializeField]
        private Button _backButton;

        [SerializeField]
        private RectTransform _context;

        [SerializeField]
        private float _totalSecondsToEnds;

        private Vector2 _contextStartPosition;
        private float _yPositionToMove;

        protected override void Awake()
        {
            base.Awake();
            _backButton.onClick.AddListener(() => OnBackButtonClick());
            SelectedGO = _backButton.gameObject;
            _contextStartPosition = _context.anchoredPosition;
        }

        public override IEnumerator Show()
        {
            yield return base.Show();
            _yPositionToMove = 0;
            foreach (var rectTransform in _context.transform.GetComponentsInChildren<RectTransform>())
            {
                _yPositionToMove += rectTransform.sizeDelta.y;
            }

            _yPositionToMove += Screen.currentResolution.height;
            StartCoroutine(Scrolling());
        }
        private IEnumerator OnBackButtonClick()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(CreditsMenuView), new CreditsEventData(CreditsEventData.CreditsEvent.Exit)));
        }

        protected override void OnCancelPressed(InputAction.CallbackContext obj)
        {
            base.OnCancelPressed(obj);
            if (IsShowing)
            {
                _backButton.onClick.Invoke();
            }
        }

        private IEnumerator Scrolling()
        {
            float t = 0;

            while (t < 1)
            {
                var newPos = new Vector2(_context.anchoredPosition.x, Mathf.Lerp(_contextStartPosition.y, _yPositionToMove, t));
                _context.anchoredPosition = newPos;
                t += Time.fixedDeltaTime * (1 / _totalSecondsToEnds);
                yield return new WaitForFixedUpdate();
            }

            _backButton.onClick.Invoke();
        }
    }
}
