using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.command.impl;
using DG.Tweening;
using UnityEngine.AddressableAssets;

namespace Chutpot.Project2D.Persistent
{
    public class PersistentStartCommand : Command
    {
#if DEVELOPMENT_BUILD || !UNITY_EDITOR
        [Inject]
        public IStoreService StoreService { get; set; }
#endif
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

        private const string _menuPrefabAddress = "MenuPrefab";

        public override void Execute()
        {
            FMODModel.MasterBus.setVolume(SettingsModel.Settings.Audio.MasterAudio);
            FMODModel.GameBus.setVolume(SettingsModel.Settings.Audio.GameAudio);
            FMODModel.MusicBus.setVolume(SettingsModel.Settings.Audio.MusicAudio);
            DOTween.Init(true, false, LogBehaviour.ErrorsOnly);

            Addressables.LoadAssetAsync<GameObject>(_menuPrefabAddress).Completed += result =>
            {
            
            };
        }
    }
}
