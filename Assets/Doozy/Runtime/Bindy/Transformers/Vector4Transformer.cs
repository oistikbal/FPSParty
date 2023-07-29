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
    /// Transforms a Vector4 value by optionally rounding its components and returning them as a formatted string.
    /// </summary>
    [CreateAssetMenu(fileName = "Vector4", menuName = "Doozy/Bindy/Transformer/Vector4", order = -950)]
    public class Vector4Transformer : ValueTransformer
    {
        public override string description => 
            "Transforms a Vector4 value by optionally rounding its components and returning them as a formatted string.";
        
        protected override Type[] fromTypes => new[] { typeof(Vector4) };
        protected override Type[] toTypes => new[] { typeof(string) };

        [SerializeField] private int DecimalPlaces = 2;
        /// <summary> The number of decimal places to round to. </summary>
        public int decimalPlaces
        {
            get => DecimalPlaces;
            set => DecimalPlaces = Mathf.Max(0, value);
        }

        [SerializeField] private bool RoundX = true;
        /// <summary> Whether to round the x component of the Vector4 value. </summary>
        public bool roundX
        {
            get => RoundX;
            set => RoundX = value;
        }

        [SerializeField] private bool RoundY = true;
        /// <summary> Whether to round the y component of the Vector4 value. </summary>
        public bool roundY
        {
            get => RoundY;
            set => RoundY = value;
        }

        [SerializeField] private bool RoundZ = true;
        /// <summary> Whether to round the z component of the Vector4 value. </summary>
        public bool roundZ
        {
            get => RoundZ;
            set => RoundZ = value;
        }

        [SerializeField] private bool RoundW = true;
        /// <summary> Whether to round the w component of the Vector4 value. </summary>
        public bool roundW
        {
            get => RoundW;
            set => RoundW = value;
        }

        [SerializeField] private string DisplayFormat = "({0}, {1}, {2}, {3})";
        /// <summary> The format string to use for formatting the Vector4 value. </summary>
        public string displayFormat
        {
            get => DisplayFormat;
            set => DisplayFormat = value;
        }
        
        /// <summary>
        /// Transforms a Vector4 value before it is displayed in a UI component.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (!enabled) return source;

            if (!(source is Vector4 vector)) return source;

            int digits = Mathf.Clamp(decimalPlaces, 0, 10);

            float x = roundX ? (float)Math.Round(vector.x, digits) : vector.x;
            float y = roundY ? (float)Math.Round(vector.y, digits) : vector.y;
            float z = roundZ ? (float)Math.Round(vector.z, digits) : vector.z;
            float w = roundW ? (float)Math.Round(vector.w, digits) : vector.w;

            return string.Format(displayFormat, x, y, z, w);
        }
    }
}
