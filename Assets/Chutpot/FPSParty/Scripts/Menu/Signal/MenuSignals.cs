using Chutpot.FPSParty.Persistent;
using strange.extensions.signal.impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chutpot.FPSParty.Menu
{
    public class MenuHideSignal : Signal<MenuEvent> { }
    public class MenuShowSignal : Signal<MenuEvent> { }
    public class MenuPointerSignal : Signal<GameObject> { }
    public class MenuPreHideSignal : Signal { }
    public class MenuAudioExitSignal : Signal<Audio> { }
    public class MenuAudioSignal : Signal<Audio> { }
    public class MenuExitSignal : Signal { }
    public class MenuGameSignal : Signal<Persistent.Game> { }
    public class MenuLanguageSignal : Signal<int> { }
    public class MenuPlaySignal : Signal<PlayEventData> { }
    public class MenuSetSettignsSignal : Signal { }
    public class MenuStartSignal : Signal { }
    public class MenuVideoSignal : Signal<Video> { }

}
