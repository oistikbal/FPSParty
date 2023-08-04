using Chutpot.FPSParty;
using Doozy.Runtime.Signals;
using strange.extensions.context.api;
using strange.extensions.mediation.impl;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Chutpot.FPSParty.Persistent
{
    public class InputService
    {
        [Inject(ContextKeys.CONTEXT)]
        public IContext Context { get; set; }
        [Inject]
        public EventSystemModel EventSystemModel { get; set; }
        [Inject]
        public ISettingsService SettingsService { get; set; }
        [Inject]
        public InputModel InputModel { get; set; }
        [Inject]
        public SettingsModel SettingsModel { get; set; }

        private PlayerActionMap _playerActionMap;

        private const string _eventSystemAddress = "EventSystem";

        [PostConstruct]
        public void Initialize() 
        {
            _playerActionMap = new PlayerActionMap();
            //_playerMap.LoadBindingOverridesFromJson(SettingsModel.Settings.Input);

            InputModel.PlayerActionMap = _playerActionMap;

            var handle = Addressables.LoadAssetAsync<GameObject>(_eventSystemAddress);
            var op = handle.WaitForCompletion();
            var go = MonoBehaviour.Instantiate(handle.Result);
            Context.AddView(go.GetComponent<View>());
            EventSystemModel.EventSystem = go.GetComponent<EventSystem>();
            EventSystemModel.EventSystem.GetComponent<InputSystemUIInputModule>().actionsAsset = _playerActionMap.asset;

            SignalStream.Get("MainMenuUI", "Background").OnSignal += OnBackground;
        }

        private void BindKeyboard(string bindName, Action<InputActionRebindingExtensions.RebindingOperation> handle)
        {
            int index = _playerActionMap.Player.Movement.bindings.IndexOf(b => b.name == bindName);

            _playerActionMap.Player.Movement.PerformInteractiveRebinding()
                .WithBindingGroup("Keyboard")
                .WithControlsHavingToMatchPath("<Keyboard>")
                .WithCancelingThrough("<Keyboard>/escape")
                .WithTargetBinding(index)
                .Start()
                .OnComplete(handle)
                .OnCancel(handle);
        }

        private void BindController(string bindName, Action<InputActionRebindingExtensions.RebindingOperation> handle)
        {
            int index = _playerActionMap.Player.Movement.bindings.IndexOf(b => b.name == bindName);

            _playerActionMap.Player.Movement.PerformInteractiveRebinding()
                .WithBindingGroup("Gamepad")
                .WithControlsHavingToMatchPath("<Gamepad>")
                .WithCancelingThrough("<Gamepad>/buttonEast")
                .WithTargetBinding(index)
                .Start()
                .OnComplete(handle)
                .OnCancel(handle);
        }

        public void SetSelectedGO(GameObject go) 
        {
            EventSystemModel.EventSystem.SetSelectedGameObject(go);
        }

        public void OnBackground(Signal signal)
        {
            signal.TryGetValue<bool>(out bool isBackgroundOpen);
            if (isBackgroundOpen)
            {
                _playerActionMap.UI.Enable();
                _playerActionMap.Player.Disable();
            }
            else
            {
                _playerActionMap.UI.Disable();
                _playerActionMap.Player.Enable();
            }

        }
    }
}
