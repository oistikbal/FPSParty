using UnityEngine;
using UnityEngine.AddressableAssets;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using Chutpot.FPSParty.Persistent;
using strange.extensions.injector.impl;
using System.Collections.Generic;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;
using strange.extensions.mediation.impl;

namespace Chutpot.FPSParty.Game
{
    public class GameStartCommand : Command
    {
        [Inject(ContextKeys.CONTEXT)]
        public IContext Context { get; set; }
        [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject ContextView { get; set; }
        [Inject]
        public CameraService CameraService { get; set; }
        [Inject]
        public InputModel InputModel { get; set; }
        [Inject]
        public GameSettingsModel GameSettingsModel { get; set; }

        public override void Execute()
        {
            //Retain();
            /*
            Addressables.LoadSceneAsync("Chapter" + GameSettingsModel.Game.ChapterIndex, UnityEngine.SceneManagement.LoadSceneMode.Additive).Completed += chapterHandle =>
            {
                var chapterView = MonoBehaviour.FindObjectOfType<ChapterView>();
                var checkpointsView = MonoBehaviour.FindObjectOfType<CheckpointsView>();
                ChapterModel.ChapterView = chapterView;
                ChapterModel.CheckpointsView = checkpointsView;
                Context.AddView(chapterView);
                GameObject playerPrefab = null;
                GameObject waypointPrefab = null;

                //Player need to be loaded and instantiated before waypoint, error eccours when played in build or use existing build with addressables
                Addressables.LoadAssetsAsync<GameObject>(new List<string> { _playerAddress, _waypointAddress }, go => 
                {

                    switch (go.name)
                    {
                        case "Player":
                            {
                                playerPrefab = go;
                                break;
                            }
                        case "Waypoint":
                            {
                                waypointPrefab = go;
                                break;
                            }

                    }
                }, Addressables.MergeMode.Union).Completed += handle => 
                {
                    var player = MonoBehaviour.Instantiate(playerPrefab, checkpointsView.CheckPoints[GameSettingsModel.Game.ChapterCheckpoint], Quaternion.identity);
                    Context.AddView(player.transform.GetComponent<View>());
                    PlayerModel.Player = player;

                    var waypoint = MonoBehaviour.Instantiate(waypointPrefab, checkpointsView.CheckPoints[GameSettingsModel.Game.ChapterCheckpoint], Quaternion.identity);
                    Context.AddView(waypoint.transform.GetComponent<View>());
                    PlayerModel.Waypoint = waypoint;

                    GameInstantiated.Dispatch();
                    Release();
                };
                
            };
            */
        }
    }
}
