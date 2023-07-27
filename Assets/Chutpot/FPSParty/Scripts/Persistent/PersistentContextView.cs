using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.context.impl;

namespace Chutpot.FPSParty.Persistent
{
    public class PersistentContextView : ContextView
    {
        private void Awake()
        {
            context = new PersistentContext(this, true);
            context.Start();
        }
    }
}
