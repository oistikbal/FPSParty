using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.command.impl;
using strange.extensions.context.api;

namespace Chutpot.Project2D.Persistent
{
    public class SaveSettingsCommand : Command
    {
        [Inject]
        public ISettingsService SettingsService { get; set; }

        public override void Execute()
        {
            SettingsService.Write();
        }
    }
}
