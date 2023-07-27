using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

namespace Chutpot.FPSParty.Persistent
{
    public class FMODModel
    {
        public FMOD.Studio.Bus MasterBus { get; set; }
        public FMOD.Studio.Bus MusicBus { get; set; }
        public FMOD.Studio.Bus GameBus { get; set; }
    }
}
