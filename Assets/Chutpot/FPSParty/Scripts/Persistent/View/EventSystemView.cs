using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using strange.extensions.mediation.impl;

namespace Chutpot.FPSParty.Persistent
{
    public class EventSystemView : View
    {
        public EventSystem EventSystem { get; protected set; }


        protected override void Awake()
        {
            base.Awake();

            EventSystem = GetComponent<EventSystem>();
        }

        protected override void Start()
        {
            base.Start();
        }
    }
}
