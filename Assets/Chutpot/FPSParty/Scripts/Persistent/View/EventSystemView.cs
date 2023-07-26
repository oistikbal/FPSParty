using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using strange.extensions.mediation.impl;

namespace Chutpot.Project2D.Persistent
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
