using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.context.impl;

namespace Chutpot.FPSParty.Menu
{
    public class MenuContextView : ContextView
    {
        private void Awake()
        {
            context = new MenuContext(this, true);
            context.Start();
        }
    }
}
