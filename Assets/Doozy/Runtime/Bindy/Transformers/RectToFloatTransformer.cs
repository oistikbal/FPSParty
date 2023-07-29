// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UseNegatedPatternInIsExpression

namespace Doozy.Runtime.Bindy.Transformers
{
    /// <summary>
    /// Transforms a Rect value by returning either the x, y, width or height component as a float with the option to round to a specified number of decimal places.
    /// </summary>
    [CreateAssetMenu(fileName = "Rect to Float", menuName = "Doozy/Bindy/Transformer/Rect to Float", order = -950)]
    public class RectToFloatTransformer : ValueTransformer
    {
        public override string description => 
            "Transforms a Rect value by returning either the x, y, width or height component as a float with the option to round to a specified number of decimal places.";
        
        protected override Type[] fromTypes => new[] { typeof(Rect) };
        protected override Type[] toTypes => new[] { typeof(float) };

        /// <summary>
        /// Describes the component to use when formatting the Rect value.
        /// </summary>
        public enum RectComponent
        {
            /// <summary>
            /// Use the x component of the Rect value.
            /// </summary>
            X,

            /// <summary>
            /// Use the y component of the Rect value.
            /// </summary>
            Y,

            /// <summary>
            /// Use the width component of the Rect value.
            /// </summary>
            Width,

            /// <summary>
            /// Use the height component of the Rect value.
            /// </summary>
            Height
        }

        [SerializeField] private RectComponent Component = RectComponent.X;
        /// <summary>
        /// The component to use when formatting the Rect value.
        /// </summary>
        public RectComponent component
        {
            get => Component;
            set => Component = value;
        }

        [SerializeField] private int DecimalPlaces;
        /// <summary> The number of decimal places to round the output value to. </summary>
        public int decimalPlaces
        {
            get => DecimalPlaces;
            set => DecimalPlaces = value;
        }

        /// <summary>
        /// Transforms a Rect value by returning either the x, y, width or height component as a float with the option to round to a specified number of decimal places.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {   
            if (source == null) return null;
            if (!(source is Rect rect)) return source;
            if (!enabled) return source;

            float outputValue = 0;

            switch (component)
            {
                case RectComponent.X:
                    outputValue = rect.x;
                    break;
                case RectComponent.Y:
                    outputValue = rect.y;
                    break;
                case RectComponent.Width:
                    outputValue = rect.width;
                    break;
                case RectComponent.Height:
                    outputValue = rect.height;
                    break;
                // ReSharper disable once RedundantEmptySwitchSection
                default:
                    break;
            }

            if (decimalPlaces <= 0)
                return outputValue;
            
            int digits= Mathf.Clamp(decimalPlaces, 0, 10);
            outputValue = (float)Math.Round(outputValue, digits);

            return outputValue;
        }
    }
}