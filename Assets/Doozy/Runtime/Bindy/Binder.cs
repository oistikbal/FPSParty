// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Runtime.Common.Utils;
using UnityEngine;
// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Bindy
{
    [AddComponentMenu("Doozy/Bindy/Binder")]
    public class Binder : MonoBehaviour
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Bindy/Binder", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<Binder>("Binder", false, true);
        }
        #endif

        private Bind m_Bind;
        /// <summary> The Bind object that this Binder is using </summary>
        public Bind bind => m_Bind;

        [SerializeField] private BindId BindId;
        /// <summary> The BindId that this Binder used be connect to or use to create a new Bind object </summary>
        public BindId bindId => BindId;

        [SerializeField] private List<Bindable> Bindables = new();
        public List<Bindable> bindables => Bindables;

        private bool m_IsInitialized;

        private void Awake()
        {
            //connect to an existing Bind object (if one doesn't exist, a new one will be created)
            m_Bind = Bind.GetBind(BindId);
        }

        private void OnEnable()
        {
            AddBindablesToBind();
        }

        private void OnDisable()
        {
            RemoveBindablesFromBind();
        }

        private void InitializeBindables()
        {
            if (m_IsInitialized) return;
            m_IsInitialized = true;

            for (int i = 0; i < bindables.Count; i++)
            {
                Bindable b = bindables[i];
                if (b != null) continue;
                bindables.RemoveAt(i);
            }

            for (int i = bindables.Count - 1; i >= 0; i--)
            {
                Bindable b = bindables[i];
                b.Initialize();
                b.gameObject = gameObject;
                if (b.bindyValue.IsValid()) continue;
                b.gameObject = null;
                bindables.RemoveAt(i);
            }
        }

        private void AddBindablesToBind()
        {
            if (bind == null) return;
            InitializeBindables();
            for (int i = bindables.Count - 1; i >= 0; i--)
            {
                Bindable b = bindables[i];
                if (b == null) continue;
                bind.AddBindable(b);
                b.gameObject = gameObject;
                b.StartTicking();
            }
        }

        private void RemoveBindablesFromBind()
        {
            if (bind == null) return;
            for (int i = bindables.Count - 1; i >= 0; i--)
            {
                Bindable b = bindables[i];
                if (b == null) continue;
                b.gameObject = null;
                b.StopTicking();
                bind.RemoveBindable(b);
            }
        }
    }
}
