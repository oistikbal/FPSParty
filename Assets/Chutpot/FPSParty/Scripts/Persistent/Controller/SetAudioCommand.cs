using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.command.impl;
using strange.extensions.context.api;

namespace Chutpot.Project2D.Persistent
{
    public class SetAudioCommand : Command
    {
        [Inject]
        public FMODModel FMODModel { get; set; }

        [Inject]
        public Audio Audio { get; set; }

        public override void Execute()
        {
            FMODModel.MasterBus.setVolume(Audio.MasterAudio / 10f);
            FMODModel.MusicBus.setVolume(Audio.MusicAudio / 10f);
            FMODModel.GameBus.setVolume(Audio.GameAudio / 10f);
        }
    }
}
