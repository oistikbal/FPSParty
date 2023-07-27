using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.command.impl;
using Chutpot.FPSParty.Persistent;

namespace Chutpot.FPSParty.Menu
{
    public class PlayOKSoundCommand : Command
    {
        [Inject]
        public FMODService FMODService { get; set; }

        public override void Execute()
        {
            FMODService.PlayOneShot("event:/UI/UI_OK");    
        }
    }
}
