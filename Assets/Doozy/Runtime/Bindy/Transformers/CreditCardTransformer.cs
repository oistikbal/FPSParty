// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Text.RegularExpressions;
using UnityEngine;
// ReSharper disable InconsistentNaming
namespace Doozy.Runtime.Bindy.Transformers
{
    /// <summary>
    /// Transforms credit card numbers in a consistent and human-readable format,
    /// with options for different card types and number formats.
    /// </summary>
    [CreateAssetMenu(fileName = "Credit Card", menuName = "Doozy/Bindy/Transformer/Credit Card", order = -950)]
    public class CreditCardTransformer : ValueTransformer
    {
        public override string description =>
            "Transforms credit card numbers in a consistent and human-readable format, " +
            "with options for different card types and number formats.";

        protected override Type[] fromTypes => new[] { typeof(string) };
        protected override Type[] toTypes => new[] { typeof(string) };

        private CreditCardType cardType { get; set; }
        private CreditCardFormat cardFormat { get; set; }

        /// <summary>
        /// Transforms a credit card number in a consistent and human-readable format.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (source.GetType() != typeof(string)) return source;
            if (!enabled) return source;
            string stringValue = (string)source;
            CreditCardType cardType = GetCardType(stringValue);
            CreditCardFormat format = GetFormat(cardType);
            string formatted = FormatCardNumber(stringValue, format);
            return formatted;
        }

        /// <summary>
        /// Returns the type of credit card based on the first few digits of the card number.
        /// </summary>
        /// <param name="input"> The credit card number </param>
        /// <returns> The type of credit card </returns>
        private static CreditCardType GetCardType(string input)
        {
            var visaPattern = new Regex(@"^4[0-9]{12}(?:[0-9]{3})?$");
            var mastercardPattern = new Regex(@"^5[1-5][0-9]{14}$");
            var amexPattern = new Regex(@"^3[47][0-9]{13}$");
            var discoverPattern = new Regex(@"^6(?:011|5[0-9]{2})[0-9]{12}$");
            var dinersClubPattern = new Regex(@"^3(?:0[0-5]|[68][0-9])[0-9]{11}$");
            var jcbPattern = new Regex(@"^(?:2131|1800|35\d{3})\d{11}$");
            var maestroPattern = new Regex(@"^(?:5[0678]\d\d|6304|6390|67\d\d)\d{8,15}$");
            var unionPayPattern = new Regex(@"^(62|88)\d{14,17}$");

            if (visaPattern.IsMatch(input))
                return CreditCardType.Visa;

            if (mastercardPattern.IsMatch(input))
                return CreditCardType.Mastercard;

            if (amexPattern.IsMatch(input))
                return CreditCardType.Amex;

            if (discoverPattern.IsMatch(input))
                return CreditCardType.Discover;

            if (dinersClubPattern.IsMatch(input))
                return CreditCardType.DinersClub;

            if (jcbPattern.IsMatch(input))
                return CreditCardType.JCB;

            if (maestroPattern.IsMatch(input))
                return CreditCardType.Maestro;

            if (unionPayPattern.IsMatch(input))
                return CreditCardType.UnionPay;

            return CreditCardType.Unknown;
        }

        /// <summary>
        /// Returns the format to use for the given credit card type.
        /// </summary>
        /// <param name="cardType"> The type of credit card </param>
        /// <returns> The format to use for the given credit card type </returns>
        private static CreditCardFormat GetFormat(CreditCardType cardType)
        {
            switch (cardType)
            {
                case CreditCardType.Amex:
                    return CreditCardFormat.FourGroupsOfFour;
                case CreditCardType.Discover:
                    return CreditCardFormat.FourGroupsOfFour;
                case CreditCardType.Mastercard:
                    return CreditCardFormat.FourGroupsOfFour;
                case CreditCardType.Visa:
                    return CreditCardFormat.FourGroupsOfFour;
                case CreditCardType.DinersClub:
                    return CreditCardFormat.TwoGroupsOfSix;
                case CreditCardType.JCB:
                    return CreditCardFormat.FourGroupsOfFour;
                case CreditCardType.Maestro:
                    return CreditCardFormat.FourGroupsOfFour;
                case CreditCardType.UnionPay:
                    return CreditCardFormat.FourGroupsOfFive;
                case CreditCardType.Unknown:
                default:
                    return CreditCardFormat.FourGroupsOfFour;
            }
        }

        /// <summary>
        /// Formats a credit card number in a consistent and human-readable format.
        /// </summary>
        /// <param name="input"> The credit card number to format </param>
        /// <param name="format"> The format to use when formatting the credit card number </param>
        /// <returns> The formatted credit card number </returns>
        private static string FormatCardNumber(string input, CreditCardFormat format)
        {
            string pattern = format switch
                             {
                                 CreditCardFormat.FourGroupsOfFour => "#### #### #### ####",
                                 CreditCardFormat.TwoGroupsOfSix   => "## ## ###### ####",
                                 CreditCardFormat.FourGroupsOfFive => "##### ##### ##### ####",
                                 _                                 => "#### #### #### ####"
                             };

            string result = "";
            int index = 0;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < pattern.Length; i++)
            {
                if (index >= input.Length)
                {
                    break;
                }

                if (pattern[i] == '#')
                {
                    result += input[index];
                    index++;
                }
                else
                {
                    result += pattern[i];
                }
            }

            return result;
        }

        /// <summary>
        /// Defines the different types of credit cards.
        /// </summary>
        private enum CreditCardType
        {
            Unknown,
            Amex,
            Discover,
            Mastercard,
            Visa,
            DinersClub,
            JCB,
            Maestro,
            UnionPay
        }

        /// <summary>
        /// Defines the different formats for credit card numbers.
        /// </summary>
        private enum CreditCardFormat
        {
            FourGroupsOfFour,
            TwoGroupsOfSix,
            FourGroupsOfFive
        }
    }
}
