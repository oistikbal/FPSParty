#if STEAM_STORE
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chutpot.Project2D.Persistent
{
    public class SteamStoreService : IStoreService
    {
        private GameObject _steamStoreView;


        [PostConstruct]
        public void Initialize()
        {
            if (!Auth())
            {
                Application.Quit();
            }

            _steamStoreView = new GameObject("SteamStoreView", new[]{ typeof(SteamStoreView) });
#if DEVELOPMENT_BUILD
            RegisterDebugCallbacks();
#endif
        }

        ~SteamStoreService() 
        {
            Steamworks.SteamClient.Shutdown();
        }

        public bool Auth()
        {
            try
            {
                Steamworks.SteamClient.Init(10);
                return true;
            }
            catch (System.Exception e)
            {
                // Something went wrong - it's one of these:
                //
                //     Steam is closed?
                //     Can't find steam_api dll?
                //     Don't have permission to play app?
                //
                Debug.LogException(e);
                return false;
            }
        }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private void RegisterDebugCallbacks() 
        {
            Steamworks.Dispatch.OnDebugCallback = (type, str, server) =>
            {
                Console.WriteLine($"[Callback {type} {(server ? "server" : "client")}]");
                Console.WriteLine(str);
                Console.WriteLine($"");
            };

            Steamworks.Dispatch.OnException = (e) =>
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
            };
        }
#endif
    }
}
#endif