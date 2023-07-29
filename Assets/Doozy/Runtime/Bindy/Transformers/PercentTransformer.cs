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
    /// Transforms a float value as a percentage, where 1.0 is represented as 100%.
    /// <para/> Converts a value in the range 0-1 to a percentage in the range 0-100, or converts a percentage in the range 0-100 to a value in the range 0-1.
    /// </summary>
    [CreateAssetMenu(fileName = "Percent", menuName = "Doozy/Bindy/Transformer/Percent", order = -950)]
    public class PercentTransformer : ValueTransformer
    {
        public override string description => 
            "Transforms a float value as a percentage, where 1.0 is represented as 100%.\n" +
            "Converts a value in the range 0-1 to a percentage in the range 0-100, " +
            "or converts a percentage in the range 0-100 to a value in the range 0-1.";
        
        protected override Type[] fromTypes => new[] { typeof(decimal), typeof(double), typeof(float), typeof(int) };
        protected override Type[] toTypes => new[] { typeof(float) };

        /// <summary>
        /// Describes the ways the percent formatter can operate.
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Converts a value in the range 0-1 to a percentage in the range 0-100.
            /// </summary>
            ValueToPercent,

            /// <summary>
            /// Converts a percentage in the range 0-100 to a value in the range 0-1.
            /// </summary>
            PercentToValue
        }

        [SerializeField] private Mode FormatMode = Mode.ValueToPercent;
        /// <summary>
        /// The mode in which the formatter will operate.
        /// <para/> ValueToPercent - Converts a value in the range 0-1 to a percentage in the range 0-100.
        /// <para/> PercentToValue - Converts a percentage in the range 0-100 to a value in the range 0-1.
        /// </summary>
        public Mode formatMode
        {
            get => FormatMode;
            set => FormatMode = value;
        }

        [SerializeField] private string PercentSymbol = "%";
        /// <summary>
        /// The symbol that will be appended to the formatted value.
        /// </summary>
        public string percentSymbol
        {
            get => PercentSymbol;
            set => PercentSymbol = value;
        }

        [SerializeField] private bool AppendPercentSymbol = true;
        /// <summary>
        /// If TRUE, the percent symbol will be appended to the formatted value.
        /// </summary>
        public bool appendPercentSymbol
        {
            get => AppendPercentSymbol;
            set => AppendPercentSymbol = value;
        }

        [SerializeField] private string NegativeSymbol = "-";
        /// <summary>
        /// The symbol that will be prepended to the formatted value if it is negative.
        /// </summary>
        public string negativeSymbol
        {
            get => NegativeSymbol;
            set => NegativeSymbol = value;
        }

        [SerializeField] private bool AllowNegativePercentages;
        /// <summary>
        /// If TRUE, negative percentages will be allowed.
        /// </summary>
        public bool allowNegativePercentages
        {
            get => AllowNegativePercentages;
            set => AllowNegativePercentages = value;
        }

        [SerializeField] private float MinPercentValue;
        /// <summary>
        /// The minimum value for percentages. Values below this will be clamped.
        /// </summary>
        public float minPercentValue
        {
            get => MinPercentValue;
            set => MinPercentValue = value;
        }

        [SerializeField] private float MaxPercentValue = 100f;
        /// <summary>
        /// The maximum value for percentages. Values above this will be clamped.
        /// </summary>
        public float maxPercentValue
        {
            get => MaxPercentValue;
            set => MaxPercentValue = value;
        }

        [SerializeField] private bool UseMinMaxPercentValues;
        /// <summary>
        /// If TRUE, the minimum and maximum percent values will be used when formatting the value.
        /// </summary>
        public bool useMinMaxPercentValues
        {
            get => UseMinMaxPercentValues;
            set => UseMinMaxPercentValues = value;
        }

        [SerializeField] private bool RoundToNearest;
        /// <summary>
        /// If TRUE, values will be rounded to the nearest decimal place.
        /// </summary>
        public bool roundToNearest
        {
            get => RoundToNearest;
            set => RoundToNearest = value;
        }

        [SerializeField] private string DecimalSeparator = ".";
        /// <summary>
        /// The decimal separator that will be used when formatting the value.
        /// </summary>
        public string decimalSeparator
        {
            get => DecimalSeparator;
            set => DecimalSeparator = value;
        }

        [SerializeField] private int DecimalPlaces;
        /// <summary>
        /// The number of decimal places that will be used when formatting the value.
        /// </summary>
        public int decimalPlaces
        {
            get => DecimalPlaces;
            set => DecimalPlaces = value;
        }

        [SerializeField] private string FormatString = "0.00";
        /// <summary>
        /// The format string that will be used when formatting the value.
        /// </summary>
        public string formatString
        {
            get => FormatString;
            set => FormatString = value;
        }

        [SerializeField] private bool UseFormatString;
        /// <summary>
        /// If TRUE, the format string will be used when formatting the value.
        /// </summary>
        public bool useFormatString
        {
            get => UseFormatString;
            set => UseFormatString = value;
        }
        
        /// <summary>
        /// Transforms a value before it is displayed in a UI component.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (!enabled) return source;

            float floatValue = Convert.ToSingle(source);

            // Convert value to percentage if necessary
            if (formatMode == Mode.ValueToPercent)
                floatValue *= 100f;

            // Clamp value between min and max percent values
            floatValue =
                useMinMaxPercentValues //&& formatMode == Mode.ValueToPercent
                    ? Mathf.Clamp(floatValue, minPercentValue, maxPercentValue)
                    : floatValue;

            int digits = Mathf.Clamp(decimalPlaces, 0, 12);

            // Round value
            floatValue =
                roundToNearest
                    ? (float)Math.Round(floatValue, digits)
                    : floatValue;

            // Add negative symbol if necessary
            floatValue = !allowNegativePercentages ? Mathf.Abs(floatValue) : floatValue;

            // Append percent symbol if necessary
            string formattedValue = floatValue.ToString(useFormatString ? formatString : "F" + digits);

            if (appendPercentSymbol)
                formattedValue += percentSymbol;

            return formattedValue;
        }

    }
}
