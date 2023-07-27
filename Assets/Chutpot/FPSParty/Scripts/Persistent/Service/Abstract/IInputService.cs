using System;
using UnityEngine;

namespace Chutpot.FPSParty.Persistent
{
    public interface IInputService
    {
        public void SetSelectedGO(GameObject go);
        public void SetDefaultInputActive(bool isActive);
        public void SetPlayerInputActive(bool isActive);
    }
}
