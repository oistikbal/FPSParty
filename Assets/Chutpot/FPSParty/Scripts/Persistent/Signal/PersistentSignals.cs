using strange.extensions.signal.impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chutpot.FPSParty.Persistent
{
    public class StartSignal : Signal { }
    public class SettingsSignal : Signal<Settings> { }

    public enum LoadingScreenStatus
    {
        Awake,
        In,
        Out
    }

    public class LoadingScreenSignal : Signal<LoadingScreenStatus> { }
    public class GameSettingsSignal : Signal { }
    public class CameraSignal : Signal<bool> { }
}
