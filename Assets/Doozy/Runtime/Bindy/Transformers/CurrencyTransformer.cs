// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Globalization;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Bindy.Transformers
{
    /// <summary>
    /// Transforms a number as a currency string. For example, 1234.56 becomes $1,234.56.
    /// </summary>
    [CreateAssetMenu(fileName = "Currency", menuName = "Doozy/Bindy/Transformer/Currency", order = -950)]
    public class CurrencyTransformer : ValueTransformer
    {
        public override string description =>
            "Transforms a number as a currency string. For example, 1234.56 becomes $1,234.56.";

        protected override Type[] fromTypes => new[] { typeof(decimal), typeof(double), typeof(float), typeof(int) };
        protected override Type[] toTypes => new[] { typeof(string) };

        public enum CurrencySymbolPosition
        {
            Before,
            After
        }

        [SerializeField] private CurrencySymbolPosition SymbolPosition = CurrencySymbolPosition.Before;
        /// <summary> The position of the currency symbol. </summary>
        public CurrencySymbolPosition symbolPosition
        {
            get => SymbolPosition;
            set => SymbolPosition = value;
        }

        [SerializeField] private string CurrencySymbol = "$";
        /// <summary>  The currency symbol to use. </summary>
        public string currencySymbol
        {
            get => CurrencySymbol;
            set => CurrencySymbol = value;
        }

        [SerializeField] private string GroupSeparator = ",";
        /// <summary> The group separator to use. </summary>
        public string groupSeparator
        {
            get => GroupSeparator;
            set => GroupSeparator = value;
        }

        [SerializeField] private string DecimalSeparator = ".";
        /// <summary> The decimal separator to use. </summary>
        public string decimalSeparator
        {
            get => DecimalSeparator;
            set => DecimalSeparator = value;
        }

        [SerializeField] private int DecimalDigits = 2;
        /// <summary> The number of decimal digits to use. </summary>
        public int decimalDigits
        {
            get => DecimalDigits;
            set => DecimalDigits = value;
        }

        /// <summary>
        /// Transforms a number value as a currency string.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (!enabled) return source;

            // data validation and sanitization
            decimalDigits = Mathf.Clamp(decimalDigits, 0, 10);

            // fix null or empty values
            if (string.IsNullOrEmpty(groupSeparator)) groupSeparator = ",";
            if (string.IsNullOrEmpty(decimalSeparator)) decimalSeparator = ".";
            
            // validate group and decimal separators
            if (groupSeparator == decimalSeparator)
                groupSeparator = groupSeparator == "," ? "." : ",";

            var numberFormat = new NumberFormatInfo
            {
                CurrencySymbol = currencySymbol,
                CurrencyGroupSeparator = groupSeparator,
                CurrencyDecimalSeparator = decimalSeparator,
                CurrencyDecimalDigits = decimalDigits
            };

            // determine symbol position based on the 'symbolPosition' property
            if (symbolPosition == CurrencySymbolPosition.Before)
            {
                return Convert.ToDecimal(source).ToString("C", numberFormat);
            }
            else
            {
                string result = Convert.ToDecimal(source).ToString("C", numberFormat);
                return result.Replace(numberFormat.CurrencySymbol, "") + numberFormat.CurrencySymbol;
            }
        }
    }
}
