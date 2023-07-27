using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.command.impl;

namespace Chutpot.FPSParty.Persistent
{
    public class EnableInputCommand : Command
    {
        [Inject]
        public IInputService InputService { get; set; }


        public override void Execute()
        {
            InputService.SetDefaultInputActive(true);
        }
    }
}
