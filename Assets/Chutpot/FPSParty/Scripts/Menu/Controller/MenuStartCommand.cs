using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using Chutpot.FPSParty.Persistent;
using strange.extensions.mediation.impl;

namespace Chutpot.FPSParty.Menu
{
    public class MenuStartCommand : Command
    {
        [Inject(ContextKeys.CONTEXT)]
        public IContext Context { get; set; }
        [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject ContextView { get; set; }
        [Inject]
        public InputModel InputModel { get; set; }
        [Inject]
        public SettingsModel SettingsModel { get; set; }

        private const string _mainCanvasAddress = "MainMenuCanvasView";

        public override void Execute()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(_mainCanvasAddress);
            var op = handle.WaitForCompletion();
            var go = MonoBehaviour.Instantiate(handle.Result);
            Context.AddView(go.GetComponent<View>());
            //need refactoring
            foreach(var menuView in go.GetComponentsInChildren<MenuView>())
            {
                menuView.PlayerActionMap = InputModel.PlayerActionMap;
            }
            Addressables.Release(handle);
        }
    }
}
