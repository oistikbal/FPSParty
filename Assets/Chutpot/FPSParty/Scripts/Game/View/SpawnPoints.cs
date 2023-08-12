using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Chutpot.FPSParty.Game
{
    public class SpawnPoints : MonoBehaviour
    {
        [HideInInspector]
        public Transform[] Points;

        private void Awake()
        {
            Points = GetComponentsInChildren<Transform>();
        }
    }
}
    