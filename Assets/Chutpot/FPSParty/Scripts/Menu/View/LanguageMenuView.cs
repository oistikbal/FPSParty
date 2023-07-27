using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using strange.extensions.mediation.impl;
using UnityEngine.Localization.Settings;
using UnityEngine.EventSystems;
using System;
using TMPro;

namespace Chutpot.FPSParty.Menu
{
    public class LanguageMenuView : MenuView
    {
        [SerializeField]
        private Button _backButton;

        [SerializeField]
        private Slider _languageSlider;

        [SerializeField]
        private ILocalesProvider _localesProvider;

        [SerializeField]
        private TextMeshProUGUI _localizationText;

        public MenuLanguageSignal MenuLanguageSignal { get; protected set; }

        protected override void Awake()
        {
            base.Awake();
            SelectedGO = _languageSlider.gameObject;
            MenuLanguageSignal = new MenuLanguageSignal();
            _backButton.onClick.AddListener(() => StartCoroutine(OnBackButtonPressed()));
            _localesProvider = LocalizationSettings.AvailableLocales;
            _languageSlider.maxValue = _localesProvider.Locales.Count - 1;

            _languageSlider.onValueChanged.AddListener(OnLanguageSliderValueChanged);

            for (int i = 0; i < _localesProvider.Locales.Count; ++i)
            {
                var locale = _localesProvider.Locales[i];
                if (LocalizationSettings.SelectedLocale == locale)
                {
                    _languageSlider.value = i;
                    break;
                }
            }

        }

        public override IEnumerator Show()
        {
            yield return base.Show();
            MenuShowSignal.Dispatch(new MenuShowEvent(typeof(LanguageMenuView), SelectedGO));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _localizationText.text = _localesProvider.Locales[(int)_languageSlider.value].LocaleName;
        }

        private IEnumerator OnBackButtonPressed()
        {
            yield return Hide();
            MenuHideSignal.Dispatch(new MenuHideEvent(typeof(LanguageMenuView), new LanguageEventData(LanguageEventData.LanguageEvent.Exit)));
        }

        private void OnLanguageSliderValueChanged(float value)
        {
            _localizationText.text = _localesProvider.Locales[(int)value].LocaleName;
            LocalizationSettings.SelectedLocale = _localesProvider.Locales[(int)_languageSlider.value];
            MenuLanguageSignal.Dispatch((int)_languageSlider.value);
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
