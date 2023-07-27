using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Chutpot.FPSParty.Persistent
{
    public class PlayerModel
    {
        public SteamId Id { get; set; }
        public string Name { get ; set; }
        public Steamworks.Data.Image? ProfileImage { get; set; }
    }
}
