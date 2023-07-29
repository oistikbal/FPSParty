// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
using UnityEngine.Serialization;
namespace Doozy.Runtime.Bindy.Transformers
{
    /// <summary>
    /// Transforms a color value as a hexadecimal string.
    /// <para/> For example, a color with the RGBA values of (0.5, 0.5, 0.5, 1.0) will be formatted as #808080FF.
    /// <para/> This transformer checks if the value is a Color or a Color32 and then uses ColorUtility.ToHtmlStringRGBA to format the color as a hexadecimal string.
    /// </summary>
    [CreateAssetMenu(fileName = "Color To Hex", menuName = "Doozy/Bindy/Transformer/Color to Hex", order = -950)]
    public class ColorToHexTransformer : ValueTransformer
    {
        public override string description =>
            "Transforms a color value as a hexadecimal string.\n\n" +
            "For example, a color with the RGBA values of (0.5, 0.5, 0.5, 1.0) will be formatted as #808080FF.\n\n" +
            "This transformer checks if the value is a Color or a Color32 and then uses ColorUtility.ToHtmlStringRGBA to format the color as a hexadecimal string.";
        
        protected override Type[] fromTypes => new[] { typeof(Color), typeof(Color32), typeof(string) };
        protected override Type[] toTypes => new[] { typeof(string) };

        [SerializeField] private bool IncludeHashSymbol = true;
        /// <summary>
        /// If true, the hexadecimal string will include the hash symbol (#).
        /// </summary>
        public bool includeHashSymbol
        {
            get => IncludeHashSymbol;
            set => IncludeHashSymbol = value;
        }
        
        [SerializeField] private bool ExcludeAlphaValue = true;
        /// <summary>
        /// If true, the alpha value will be excluded from the hexadecimal string. 
        /// </summary>
        public bool excludeAlphaValue
        {
            get => ExcludeAlphaValue;
            set => ExcludeAlphaValue = value;
        }
        
        /// <summary>
        /// Transforms a color value as a hexadecimal string. 
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (!enabled) return source;

            string colorString;
            
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (source is Color color)
                colorString = ColorUtility.ToHtmlStringRGBA(color);
            else if (source is Color32 color32)
                colorString = ColorUtility.ToHtmlStringRGBA(color32);
            else
                return source;

            // process the options
            if (excludeAlphaValue)
                colorString = colorString.Substring(0, colorString.Length - 2);
            if (includeHashSymbol)
                colorString = "#" + colorString;

            return colorString;
        }
    }
}
