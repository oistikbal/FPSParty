using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.context.impl;
using strange.extensions.command.impl;
using strange.extensions.command.api;
using strange.extensions.signal.impl;
using strange.extensions.context.api;
using Chutpot.FPSParty.Persistent;

namespace Chutpot.FPSParty.Game
{
    public class GameContext : PATContext
    {
        public GameContext(MonoBehaviour mono, bool autoStartup) : base(mono, autoStartup) { }

        protected override void mapBindings()
        {
            //injectionBinder.Bind<PlayerModel>().ToSingleton();

            
            ///injectionBinder.Bind<PlayerDashSignal>().ToSingleton();


            //mediationBinder.Bind<GroundSpikeView>().To<GroundSpikeMediator>();


            commandBinder.Bind<StartSignal>().To<GameStartCommand>().Once();
        }

        public override IContext Start()
        {
            base.Start();
            (injectionBinder.GetInstance<StartSignal>() as StartSignal).Dispatch();
            return this;
        }
    }
}
