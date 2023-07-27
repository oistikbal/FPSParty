using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.context.impl;
using strange.extensions.command.impl;
using strange.extensions.command.api;
using strange.extensions.signal.impl;
using strange.extensions.context.api;

namespace Chutpot.FPSParty.Persistent
{
    public class PersistentContext : PATContext
    {
        public PersistentContext(MonoBehaviour mono, bool autoStartup) : base(mono, autoStartup) { }

        protected override void mapBindings()
        {
            injectionBinder.Bind<ISettingsService>().To<SettingsService>().CrossContext().ToSingleton();
            injectionBinder.Bind<CameraService>().CrossContext().ToSingleton();
            injectionBinder.Bind<SceneService>().To<SceneService>().CrossContext().ToSingleton();
            injectionBinder.Bind<IInputService>().To<InputService>().CrossContext().ToSingleton();
            injectionBinder.Bind<FMODService>().CrossContext().ToSingleton();
            injectionBinder.Bind<GameSettingsService>().CrossContext().ToSingleton();
            injectionBinder.Bind<NetworkService>().CrossContext().ToSingleton();
            injectionBinder.Bind<EventSystemModel>().CrossContext().ToSingleton();
            injectionBinder.Bind<InputModel>().CrossContext().ToSingleton();
            injectionBinder.Bind<SettingsModel>().CrossContext().ToSingleton();
            injectionBinder.Bind<FMODModel>().CrossContext().ToSingleton();
            injectionBinder.Bind<GameSettingsModel>().CrossContext().ToSingleton();
            injectionBinder.Bind<CameraModel>().CrossContext().ToSingleton();
            injectionBinder.Bind<PlayerModel>().CrossContext().ToSingleton();

            mediationBinder.Bind<CameraView>().To<CameraMediator>();
            mediationBinder.Bind<LoadingScreenView>().To<LoadingScreenMediator>();
            mediationBinder.Bind<EventSystemView>().To<EventSystemMediator>();
            //mediationBinder.Bind<VirtualCamerasView>().To<VirtualCamerasMediator>();

            injectionBinder.Bind<LoadingScreenSignal>().ToSingleton();
            injectionBinder.Bind<SettingsSignal>().CrossContext().ToSingleton();
            injectionBinder.Bind<CameraSignal>().CrossContext().ToSingleton();

            commandBinder.Bind<StartSignal>().To<PersistentStartCommand>();
        }

        override public IContext Start()
        {
            base.Start();
            ((StartSignal)injectionBinder.GetInstance<StartSignal>()).Dispatch();
            return this;
        }
    }
}
