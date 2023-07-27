using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.command.impl;
using strange.extensions.context.api;

namespace Chutpot.FPSParty.Persistent
{
    public class SetGameCommand : Command
    {
        [Inject]
        public GameSettingsService GameSettingsService { get; set; }

        [Inject]
        public Persistent.Game Game { get; set; }

        public override void Execute()
        {
            GameSettingsService.SetGame(Game);
        }
    }
}
