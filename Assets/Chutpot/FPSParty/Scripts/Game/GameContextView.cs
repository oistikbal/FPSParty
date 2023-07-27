using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.context.impl;

namespace Chutpot.FPSParty.Game
{
    public class GameContextView : ContextView
    {
        private void Awake()
        {
            context = new GameContext(this, true);
            context.Start();
        }
    }
}
