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
    /// Transforms a float value by rounding it to a specified number of decimal places.
    /// <para/> For example, if the value is 1.2345 and the number of decimal places is 2, the value will be rounded to 1.23.
    /// </summary>
    [CreateAssetMenu(fileName = "Float Rounding", menuName = "Doozy/Bindy/Transformer/Float Rounding", order = -950)]
    public class FloatRoundingTransformer : ValueTransformer
    {
        public override string description =>
            "Transforms a float value by rounding it to a specified number of decimal places.\n\n" +
            "For example, if the value is 1.2345 and the number of decimal places is 2, the value will be rounded to 1.23.";

        protected override Type[] fromTypes => new[] { typeof(float), typeof(double) };
        protected override Type[] toTypes => new[] { typeof(float), typeof(double) };

        /// <summary>
        /// The number of decimal places to round to.
        /// </summary>
        [SerializeField] private int DecimalPlaces = 2;
        public int decimalPlaces
        {
            get => DecimalPlaces;
            set => DecimalPlaces = value;
        }

        /// <summary>
        /// Transforms a float value by rounding it to the specified number of decimal places.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (!enabled) return source;
            decimalPlaces = Mathf.Clamp(decimalPlaces, 0, 10);
            float floatValue = Convert.ToSingle(source);
            return Convert.ChangeType(Mathf.Round(floatValue * Mathf.Pow(10, decimalPlaces)) / Mathf.Pow(10, decimalPlaces), source.GetType());
        }
    }
}
