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
    /// Transforms a Vector2 value by returning either the x or y component as a float with the option to round to a specified number of decimal places.
    /// </summary>
    [CreateAssetMenu(fileName = "Vector2 To Float", menuName = "Doozy/Bindy/Transformer/Vector2 to Float", order = -950)]
    public class Vector2ToFloatTransformer : ValueTransformer
    {
        public override string description => 
            "Transforms a Vector2 value by returning either the x or y component as a float with the option to round to a specified number of decimal places.";
        
        protected override Type[] fromTypes => new[] { typeof(Vector2) };
        protected override Type[] toTypes => new[] { typeof(float) };

        [SerializeField] private Axis2D Axis = Axis2D.X;
        /// <summary>
        /// The axis to use when formatting the Vector2 value.
        /// </summary>
        public Axis2D axis
        {
            get => Axis;
            set => Axis = value;
        }

        [SerializeField] private int DecimalPlaces;
        /// <summary> The number of decimal places to round the output value to. </summary>
        public int decimalPlaces
        {
            get => DecimalPlaces;
            set => DecimalPlaces = value;
        }

        /// <summary>
        /// Transforms a Vector2 value by returning either the x or y component as a float with the option to round to a specified number of decimal places.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (!(source is Vector2 vector)) return source;
            if (!enabled) return source;

            float outputValue = 0;

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (axis)
            {
                case Axis2D.X:
                    outputValue = vector.x;
                    break;
                case Axis2D.Y:
                    outputValue = vector.y;
                    break;
            }

            int digits = Mathf.Clamp(DecimalPlaces, 0, 10);
            outputValue = DecimalPlaces > 0 ? (float)Math.Round(outputValue, digits) : outputValue;

            return outputValue;
        }
    }
}
