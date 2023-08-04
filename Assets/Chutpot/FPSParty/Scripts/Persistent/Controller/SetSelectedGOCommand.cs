using System;
using UnityEngine;
using strange.extensions.command.impl;
using Chutpot.FPSParty.Persistent;

namespace Chutpot.FPSParty.Menu
{
    public class SetSelectedGONavigationCommand : Command
    {
        [Inject]
        public InputService InputService { get; set; }


        public override void Execute()
        {
            //InputService.SetSelectedGO((MenuEvent as MenuShowEvent).SelectedGO);
        }
    }
}
