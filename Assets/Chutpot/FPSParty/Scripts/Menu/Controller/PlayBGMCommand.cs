using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using strange.extensions.command.impl;
using Chutpot.FPSParty.Persistent;

namespace Chutpot.FPSParty.Menu
{
    public class PlayBGMCommand : Command
    {
        [Inject]
        public FMODService FMODService { get; set; }

        private EventInstance _instance;

        public override void Execute()
        {
            _instance = FMODService.GetInstance("event:/UI/UI_BGM");
            _instance.start();
        }
    }
}
