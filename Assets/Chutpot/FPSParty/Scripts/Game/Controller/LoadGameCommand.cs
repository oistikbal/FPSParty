using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using Chutpot.FPSParty.Menu;
using Chutpot.FPSParty.Persistent;
using UnityEngine.AddressableAssets;

namespace Chutpot.FPSParty.Game
{
    public class LoadGameCommand : Command
    {
        [Inject]
        public PlayEventData PlayEvent { get; set; }
        [Inject]
        public SceneService SceneService { get; set; }
        [Inject]
        public GameSettingsService GameSettingsService { get; set; }

        private const string _gameAddress = "Game";

        private Persistent.Game _game;

        public override void Execute()
        {
            _game = GameSettingsService.SelectGame((SelectGame)PlayEvent.Event);
            SceneService.LoadScene(_gameAddress);
        }
    }
}
