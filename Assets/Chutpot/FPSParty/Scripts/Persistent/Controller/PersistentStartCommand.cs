using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.command.impl;
using DG.Tweening;
using UnityEngine.AddressableAssets;
using Chutpot.FPSParty.Menu;
using strange.extensions.context.impl;
using strange.extensions.mediation.impl;
using strange.extensions.context.api;
using UnityEngine.UI;
using TMPro;
using Steamworks;
using Netcode.Transports.Facepunch;
using System.Threading.Tasks;

namespace Chutpot.FPSParty.Persistent
{
    public class PersistentStartCommand : Command
    {
        [Inject(ContextKeys.CONTEXT)]
        public IContext Context { get; set; }
        [Inject]
        public ISettingsService SettingsService { get; set; }
        [Inject]
        public InputService InputService { get; set; }
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
        [Inject]
        public PlayerModel PlayerModel { get; set; }

        private const string _mainCanvasAddress = "MainMenuCanvas";
        private const string _networkServiceViewAddress = "NetworkService";

        public override void Execute()
        {
            Retain();
            FMODModel.MasterBus.setVolume(SettingsModel.Settings.Audio.MasterAudio);
            FMODModel.GameBus.setVolume(SettingsModel.Settings.Audio.GameAudio);
            FMODModel.MusicBus.setVolume(SettingsModel.Settings.Audio.MusicAudio);
            DOTween.Init(true, false, LogBehaviour.ErrorsOnly);

            var handle = Addressables.LoadAssetAsync<GameObject>(_mainCanvasAddress);
            var op = handle.WaitForCompletion();
            var go = MonoBehaviour.Instantiate(handle.Result);
            //Context.AddView(go.GetComponent<View>());
            Addressables.Release(handle);

            handle = Addressables.LoadAssetAsync<GameObject>(_networkServiceViewAddress);
            op = handle.WaitForCompletion();
            go = MonoBehaviour.Instantiate(handle.Result);
            Context.AddView(go.GetComponent<View>());
            go.GetComponent<NetworkServiceView>().Initialized += InitNetworkService;


            InitNetworkService();
        }

        private void InitNetworkService()
        {
            if (Steamworks.SteamClient.IsValid)
            {
                injectionBinder.Bind<AbstractNetworkService>().To<FPNetworkService>();
                injectionBinder.GetInstance<AbstractNetworkService>();

                var playerTag = Doozy.Runtime.UIManager.Components.UITag.GetFirstTag("MainMenuUI", "Player");
                playerTag.GetComponent<RawImage>().texture = PlayerModel.ProfileImage.Value.Covert();
                playerTag.GetComponentInChildren<TextMeshProUGUI>().text = PlayerModel.Name;
            }
            else
            {
                injectionBinder.Bind<AbstractNetworkService>().To<UTNetworkService>();
                injectionBinder.GetInstance<AbstractNetworkService>();
            }
            Release();
        }
    }
}
