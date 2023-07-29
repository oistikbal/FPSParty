// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo

namespace Doozy.Runtime.Bindy.Transformers
{
    /// <summary>
    /// Transforms a string in the format '#RRGGBB' to a color value.
    /// <para/> The string is parsed back to a color using ColorUtility.TryParseHtmlString.
    /// <para/> The format is the same as the one used by the Unity Editor.
    /// </summary>
    [CreateAssetMenu(fileName = "String to Color", menuName = "Doozy/Bindy/Transformer/String to Color", order = -950)]
    public class StringToColorTransformer : ValueTransformer
    {
        public override string description =>
            "Transforms a string in the format '#RRGGBB' to a color value.\n\n" +
            "The string is parsed back to a color using ColorUtility.TryParseHtmlString. \n\n" +
            "The format is the same as the one used by the Unity Editor.";
        
        protected override Type[] fromTypes => new[] { typeof(string) };
        protected override Type[] toTypes => new[] { typeof(Color) };

        /// <summary>
        /// Transforms a string in the format '#RRGGBB' to a color value.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (!enabled) return source;
            if (!(source is string stringValue)) return source;
            ColorUtility.TryParseHtmlString(stringValue, out Color colorValue);
            return colorValue;
        }
    }
}