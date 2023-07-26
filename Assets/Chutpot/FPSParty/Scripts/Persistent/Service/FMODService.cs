using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

namespace Chutpot.Project2D.Persistent
{
    public class FMODService
    {
        [Inject]
        public FMODModel FMODModel { get; set; }
        [Inject]
        public SettingsModel SettingsModel { get; set; }

        [PostConstruct]
        public void Initialize() 
        {
            FMODModel.MasterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
            FMODModel.MusicBus = FMODUnity.RuntimeManager.GetBus("bus:/Music");
            FMODModel.GameBus = FMODUnity.RuntimeManager.GetBus("bus:/Game");

            FMODModel.MasterBus.setVolume(SettingsModel.Settings.Audio.MasterAudio / 10f);
            FMODModel.MusicBus.setVolume(SettingsModel.Settings.Audio.MusicAudio / 10f);
            FMODModel.GameBus.setVolume(SettingsModel.Settings.Audio.GameAudio / 10f);
        }

        public void PlayOneShot(string eventName)
        {
            FMODUnity.RuntimeManager.PlayOneShot(eventName);
        }

        public EventInstance GetInstance(string eventName)
        {
            var instance = FMODUnity.RuntimeManager.CreateInstance(eventName);
            instance.start();
            return instance;
        }
    }
}
