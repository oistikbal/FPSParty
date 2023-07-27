using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.context.impl;
using strange.extensions.command.impl;
using strange.extensions.command.api;
using strange.extensions.signal.impl;
using strange.extensions.context.api;
using Chutpot.FPSParty.Persistent;
using Chutpot.FPSParty.Game;

namespace Chutpot.FPSParty.Menu
{
    public class MenuContext : PATContext
    {
        public MenuContext(MonoBehaviour mono, bool autoStartup) : base(mono, autoStartup) { }


        protected override void mapBindings()
        {

            mediationBinder.Bind<AutoSaveView>().To<AutoSaveMediator>();
            mediationBinder.Bind<CreditsMenuView>().To<CreditsMenuMediator>();
            mediationBinder.Bind<LanguageMenuView>().To<LanguageMenuMediator>();
            mediationBinder.Bind<MainMenuView>().To<MainMenuMediator>();
            mediationBinder.Bind<OptionsMenuView>().To<OptionsMenuMediator>();
            mediationBinder.Bind<PlayMenuView>().To<PlayMenuMediator>();
            mediationBinder.Bind<VideoMenuView>().To<VideoMenuMediator>();
            mediationBinder.Bind<GameMenuView>().To<GameMenuMediator>();
            mediationBinder.Bind<AudioMenuView>().To<AudioMenuMediator>();
            mediationBinder.Bind<MainMenuCanvasView>().To<MainMenuCanvasMediator>();

            injectionBinder.Bind<MenuShowSignal>().ToSingleton();
            injectionBinder.Bind<MenuHideSignal>().ToSingleton();

            commandBinder.Bind<MenuStartSignal>().To<MenuStartCommand>().To<PlayBGMCommand>().Once();
            commandBinder.Bind<MenuPlaySignal>().To<GameStartSignal>();
            commandBinder.Bind<MenuShowSignal>().To<SetSelectedGONavigationCommand>().To<EnableInputCommand>();
            commandBinder.Bind<MenuPreHideSignal>().To<PlayOKSoundCommand>().To<DisableInputCommand>();
            commandBinder.Bind<MenuPointerSignal>().To<SetSelectedGOPointerCommand>();
            commandBinder.Bind<MenuExitSignal>().To<GameExitCommand>().Once();
            commandBinder.Bind<MenuAudioSignal>().To<SetAudioCommand>();
            commandBinder.Bind<MenuGameSignal>().To<SetGameCommand>();
            commandBinder.Bind<MenuVideoSignal>().To<SetVideoCommand>();
            commandBinder.Bind<MenuSetSettignsSignal>().To<SaveSettingsCommand>();
            commandBinder.Bind<MenuAudioExitSignal>().To<SaveAudioCommand>();
        }

        public override IContext Start()
        {
            base.Start();
            (injectionBinder.GetInstance<MenuStartSignal>() as MenuStartSignal).Dispatch();
            return this;
        }
    }
}
