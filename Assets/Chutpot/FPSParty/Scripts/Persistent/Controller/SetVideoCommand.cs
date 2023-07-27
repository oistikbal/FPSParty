using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.command.impl;
using strange.extensions.context.api;

namespace Chutpot.FPSParty.Persistent
{
    public class SetVideoCommand : Command
    {
        [Inject]
        public ISettingsService SettingsService { get; set; }
        [Inject]
        public Video Video { get; set; }

        public override void Execute()
        {
            SettingsService.SetVideo(Video);
            SettingsService.Write();
        }
    }
}
