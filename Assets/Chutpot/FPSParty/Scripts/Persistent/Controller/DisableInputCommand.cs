using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.command.impl;

namespace Chutpot.Project2D.Persistent
{
    public class DisableInputCommand : Command
    {
        [Inject]
        public IInputService InputService { get; set; }


        public override void Execute()
        {
            InputService.SetDefaultInputActive(false);
        }
    }
}
