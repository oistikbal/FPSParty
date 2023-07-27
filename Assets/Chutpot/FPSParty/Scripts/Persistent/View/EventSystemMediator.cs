using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using strange.extensions.mediation.impl;

namespace Chutpot.FPSParty.Persistent
{
    public class EventSystemMediator : Mediator
    {
        [Inject]
        public EventSystemView View { get; set; }


        public override void OnRegister()
        {
            base.OnRegister();
        }

        public override void OnRemove()
        {
            base.OnRemove();
        }

    }
}
