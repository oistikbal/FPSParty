// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Bindy.Transformers
{
    /// <summary>
    /// Transforms an Enum value by returning its name or value as a string.
    /// </summary>
    [CreateAssetMenu(fileName = "Enum", menuName = "Doozy/Bindy/Transformer/Enum", order = -950)]
    public class EnumTransformer : ValueTransformer
    {
        public override string description => "Formats an Enum value by returning its name or value as a string.";
        
        protected override Type[] fromTypes => new[] { typeof(Enum) };
        protected override Type[] toTypes => new[] { typeof(string) };

        [SerializeField] private bool UseEnumName = true;
        /// <summary>
        /// If enabled, the enum name will be used as the string value.
        /// If disabled, the enum value will be used as the string value.
        /// </summary>
        public bool useEnumName
        {
            get => UseEnumName;
            set => UseEnumName = value;
        }

        /// <summary>
        /// Transforms an Enum value before it is displayed in a UI component.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (!enabled) return source;

            // ReSharper disable once InvertIf
            if (useEnumName)
            {
                if (!(source is Enum enumValue)) return source;
                return enumValue.ToString();
            }
            
            return source.ToString();
        }
    }
}