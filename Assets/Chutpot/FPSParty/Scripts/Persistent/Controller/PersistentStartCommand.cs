using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.command.impl;
using DG.Tweening;
using UnityEngine.AddressableAssets;
using Chutpot.FPSParty.Menu;

namespace Chutpot.FPSParty.Persistent
{
    public class PersistentStartCommand : Command
    {
        [Inject]
        public NetworkService NetworkService { get; set; }

        [Inject]
        public ISettingsService SettingsService { get; set; }
        [Inject]
        public IInputService InputService { get; set; }
        [Inject]
        public SceneService SceneService { get; set; }
        [Inject]
        public CameraService CameraService { get; set; }
        [Inject]
        public GameSettingsService GameSettingsService { get; set; }
        [Inject]
        public SettingsModel SettingsModel { get; set; }
        [Inject]
        public FMODModel FMODModel { get; set; }


        public override void Execute()
        {
            FMODModel.MasterBus.setVolume(SettingsModel.Settings.Audio.MasterAudio);
            FMODModel.GameBus.setVolume(SettingsModel.Settings.Audio.GameAudio);
            FMODModel.MusicBus.setVolume(SettingsModel.Settings.Audio.MusicAudio);
            DOTween.Init(true, false, LogBehaviour.ErrorsOnly);

            var menuContext = new GameObject("MainMenuContextView", new []{typeof(MenuContextView) });
        }
    }
}
