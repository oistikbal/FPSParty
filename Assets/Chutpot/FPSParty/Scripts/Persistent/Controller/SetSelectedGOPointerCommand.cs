using System;
using UnityEngine;
using strange.extensions.command.impl;
using Chutpot.FPSParty.Persistent;

namespace Chutpot.FPSParty.Menu
{
    public class SetSelectedGOPointerCommand : Command
    {
        [Inject]
        public InputService InputService { get; set; }

        [Inject]
        public GameObject GameObject { get; set; }

        public override void Execute()
        {
            InputService.SetSelectedGO(GameObject);
        }
    }
}
