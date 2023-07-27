using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.command.impl;

namespace Chutpot.FPSParty.Persistent
{
    public class SaveAudioCommand : Command
    {
        [Inject]
        public ISettingsService SettingsService { get; set; }
        [Inject]
        public Audio Audio { get; set; }

        public override void Execute()
        {
            SettingsService.SetAudio(Audio);
            SettingsService.Write();
        }
    }
}
