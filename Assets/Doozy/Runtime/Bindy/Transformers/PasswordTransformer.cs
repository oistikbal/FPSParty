// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UseNegatedPatternInIsExpression
// ReSharper disable ReplaceSubstringWithRangeIndexer

namespace Doozy.Runtime.Bindy.Transformers
{
    /// <summary>
    /// Transforms a string by masking it (e.g. "password" becomes "*******").
    /// </summary>
    [CreateAssetMenu(fileName = "Password", menuName = "Doozy/Bindy/Transformer/Password", order = -950)]
    public class PasswordTransformer : ValueTransformer
    {
        public override string description =>
            "Formats a string by masking it (e.g. \"password\" becomes \"*******\").";
        
        protected override Type[] fromTypes => new[] { typeof(string) };
        protected override Type[] toTypes => new[] { typeof(string) };

        [SerializeField] private char MaskCharacter = '*';
        /// <summary> The character to use as a mask for the password </summary>
        public char maskCharacter
        {
            get => MaskCharacter;
            set => MaskCharacter = value;
        }

        [SerializeField] private int VisibleCharacters = 3;
        /// <summary> The number of characters to leave visible at the start of the original string </summary>
        public int visibleCharacters
        {
            get => VisibleCharacters;
            set => VisibleCharacters = value;
        }

        /// <summary>
        /// Transforms a string by masking it.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (!(source is string str)) return source;
            if (!enabled) return source;

            if (visibleCharacters >= str.Length)
                return new string(maskCharacter, str.Length);

            string visible = str.Substring(0, visibleCharacters);
            string masked = new(maskCharacter, str.Length - visibleCharacters);
            return visible + masked;
        }
    }
}