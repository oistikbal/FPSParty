#if STEAM_STORE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chutpot.FPSParty
{
    //No need to register as view
    public class SteamStoreView : MonoBehaviour
    {
        void Update()
        {
            Steamworks.SteamClient.RunCallbacks();
        }
    }
}
#endif