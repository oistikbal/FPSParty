using System;
using UnityEngine;

namespace Chutpot.Project2D.Persistent
{
    public interface IInputService
    {
        public void SetSelectedGO(GameObject go);
        public void SetDefaultInputActive(bool isActive);
        public void SetPlayerInputActive(bool isActive);
    }
}
