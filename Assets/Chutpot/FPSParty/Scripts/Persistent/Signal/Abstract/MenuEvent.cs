using System;
using UnityEngine;

namespace Chutpot.FPSParty.Menu
{
    public abstract class MenuEventData { }

    public abstract class MenuEvent
    {
        public MenuEventData MenuEventData { get; protected set; }

        public MenuEvent(MenuEventData menuEventData = null) 
        {
            MenuEventData = menuEventData;
        }
    }

    public class MenuShowEvent : MenuEvent
    {
        public Type Type { get; protected set; }
        public GameObject SelectedGO { get; protected set; }

        public MenuShowEvent(Type type, GameObject selectedGO, MenuEventData data = null) : base(data)
        {
            Type = type;
            SelectedGO = selectedGO;
            MenuEventData = data;
        }
    }

    public class MenuHideEvent : MenuEvent
    {
        public Type Type { get; protected set; }

        public MenuHideEvent(Type type, MenuEventData data = null) : base(data)
        {
            Type = type;
            MenuEventData = data;
        }
    }
}
