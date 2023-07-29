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
    /// Transforms a string value by replacing a placeholder with a value.
    /// <para/> As an example, you can use this transformer to format a string value by adding a currency symbol to it.
    /// Or you can use it to format a string value by adding a prefix or a suffix to it.
    /// </summary>
    [CreateAssetMenu(fileName = "String", menuName = "Doozy/Bindy/Transformer/String", order = -950)]
    public class StringTransformer : ValueTransformer
    {
        public override string description =>
            "Transforms a string value by replacing a placeholder with a value.\n\n" +
            "As an example, you can use this transformer to format a string value by adding a currency symbol to it.\n\n" +
            "Or you can use it to format a string value by adding a prefix or a suffix to it.";
        
        protected override Type[] fromTypes => new[] { typeof(string) };
        protected override Type[] toTypes => new[] { typeof(string) };

        [SerializeField] private string StringFormat = "{0}";
        /// <summary> The format string to use for formatting the string value. </summary>
        public string stringFormat
        {
            get => StringFormat;
            set => StringFormat = value;
        }

        /// <summary>
        /// Transforms a string value before it is displayed in a UI component.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (!enabled) return source;

            string stringValue = (string)source;
            return string.Format(stringFormat, stringValue);
        }
    }
}


