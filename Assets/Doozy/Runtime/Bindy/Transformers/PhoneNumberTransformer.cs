// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UseNegatedPatternMatching
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Bindy.Transformers
{
    /// <summary>
    /// Transforms phone numbers in a consistent and human-readable format.
    /// For example, 5551234567 becomes (555) 123-4567.
    /// </summary>
    [CreateAssetMenu(fileName = "Phone Number", menuName = "Doozy/Bindy/Transformer/Phone Number", order = -950)]
    public class PhoneNumberTransformer : ValueTransformer
    {
        public override string description =>
            "Formats phone numbers in a consistent and human-readable format. " +
            "For example, 5551234567 becomes (555) 123-4567.";

        protected override Type[] fromTypes => new[] { typeof(string), typeof(int), typeof(long) };
        protected override Type[] toTypes => new[] { typeof(string) };

        /// <summary>
        /// Pattern used to format phone numbers
        /// </summary>
        public PhoneNumberPattern Pattern;

        public PhoneNumberTransformer()
        {
            Pattern = new PhoneNumberPattern();
        }

        /// <summary>
        /// Transforms a phone number before it is displayed in a UI component.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (!enabled) return source;
            string phoneNumber = source?.ToString();

            if (phoneNumber == null)
                return source;

            return Pattern.ParsePhoneNumber(phoneNumber);
        }
    }

    [Serializable]
    public class PhoneNumberPattern
    {
        public string Local;
        public string International;

        public PhoneNumberPattern()
        {
            // ReSharper disable StringLiteralTypo
            Local = "XXXXXXXXXX";
            International = "XXXXXXXXXX";
            // ReSharper restore StringLiteralTypo
        }
        
        public PhoneNumberPattern(string localPattern = "", string internationalPattern = "") : this()
        {
            Local = localPattern.IsNullOrEmpty() ? Local : localPattern;
            International = internationalPattern.IsNullOrEmpty() ? International : internationalPattern;
        }

        public string ParsePhoneNumber(string phoneNumber)
        {
            int phoneNumberDigitCount = phoneNumber.Count(char.IsDigit);
            int patternDigitCount = GetPatternDigitCount(phoneNumber);

            //remove any non-digit characters except the + sign from the phone number
            phoneNumber = Regex.Replace(phoneNumber, @"[^\d\+]", "");

            //remove ( ) - and whitespaces from the phone number
            phoneNumber = Regex.Replace(phoneNumber, @"[\(\)\-\s]", "");

            //if the phone number is empty, return an empty string
            if (phoneNumberDigitCount == 0)
                return phoneNumber;

            //wait for 3 digits to be entered before formatting the phone number to check if it's international
            if (phoneNumberDigitCount < 3)
                return phoneNumber;

            bool isInternational = IsInternational(phoneNumber);

            string countryCode = "";
            int countryCodeDigitCount = 0;
            if (isInternational && CountryCode.IsValid(phoneNumber))
            {
                phoneNumber = phoneNumber.Replace("+", "");              //remove the + sign from the phone number
                countryCode = CountryCode.GetCountryCode(phoneNumber);   //get the country code from the phone number
                phoneNumber = phoneNumber.Substring(countryCode.Length); //remove the country code from the phone number
                countryCodeDigitCount = countryCode.Count(char.IsDigit); //get the number of digits in the country code
            }

            if (phoneNumberDigitCount > patternDigitCount + countryCodeDigitCount)
                return phoneNumber;

            //check if the phone number is complete or partial
            int missingDigits = patternDigitCount - phoneNumberDigitCount - countryCodeDigitCount;
            bool isPartial = missingDigits > 0;
            //if the phone number is not complete, we need to add mock digits to the end
            if (isPartial) phoneNumber += new string('0', missingDigits);

            string regex, format;
            (regex, format) = Process(isInternational);


            string output = Regex.Replace(phoneNumber, regex, format).Trim();

            //we need to replace the mock digits with spaces
            if (isPartial)
            {
                //iterate output from the end and replace the mock digits with spaces
                for (int i = output.Length - 1; i >= 0; i--)
                {
                    if (missingDigits <= 0) break;
                    char c = output[i];
                    if (c != '0')
                        continue;
                    output = output.Remove(i, 1).Insert(i, " ");
                    missingDigits--;
                }
            }

            if (isInternational)
            {
                //add the country code to the beginning of the phone number
                output = "+" + countryCode + " " + output;
            }

            return output;
        }

        public (string regex, string format) Process(bool isInternational)
        {
            string pattern = isInternational ? International : Local;

            string regex = "";
            string format = "";
            int digitCount = 0;
            int groupCount = 0;

            foreach (char c in pattern)
            {
                if (c == 'X')
                {
                    digitCount++;
                    continue;
                }

                if (digitCount > 0)
                {
                    regex += $"(\\d{{1,{digitCount}}})?";
                    groupCount++;
                    format += "$" + groupCount;
                    digitCount = 0;
                }

                format += c;
            }

            if (digitCount <= 0)
                return (regex, format);

            regex += $"(\\d{{1,{digitCount}}})";
            groupCount++;
            format += "$" + groupCount;

            return (regex, format);
        }

        private bool IsInternational(string phoneNumber) =>
            phoneNumber.StartsWith("+");

        private int GetPatternDigitCount(string phoneNumber) =>
            Local.Count(c => c == 'X');

        public override string ToString() =>
            $"{Local}";

        public class Patterns
        {
            public static PhoneNumberPattern defaultPattern => international;
            // ReSharper disable StringLiteralTypo
            public static PhoneNumberPattern international => new("XXXXXXXXXX", "XXXXXXXXXX");
            
            public static PhoneNumberPattern argentina => new("(XXX) XXX-XXXX", "XXXXXXXXXXX");
            public static PhoneNumberPattern australia => new("XXXX XXX XXX", "XXXXXXXXXX");
            public static PhoneNumberPattern brazil => new("(XX) XXXXX-XXXX", "(XX) XXXXXX-XXXX");
            public static PhoneNumberPattern canada => new("(XXX) XXX-XXXX", "(XXX) XXX-XXXX");
            public static PhoneNumberPattern china => new("XXX XXXX XXXX", "XXX XXXX XXXX");
            public static PhoneNumberPattern france => new("XX XX XX XX XX, XXXXXXXXXX");
            public static PhoneNumberPattern germany => new("XXX XXXXXXX", "XXXXXXXXXX");
            public static PhoneNumberPattern india => new("XXXXXXXXXX", "XXXXXXXXXX");
            public static PhoneNumberPattern italy => new("XX XXXX XXXX", "XXXX XXXX XXXX");
            public static PhoneNumberPattern japan => new("XX-XXXX-XXXX", "XXX-XXXX-XXXX");
            public static PhoneNumberPattern mexico => new("XX XX XX XX XX", "XXXXXXXXXX");
            public static PhoneNumberPattern puertoRico => new("(XXX) XXX-XXXX", "(XXX) XXX-XXXX");
            public static PhoneNumberPattern russia => new("(XXX) XXX-XXXX", "(XXX) XXX-XXXX");
            public static PhoneNumberPattern southKorea => new("XX-XXX-XXXX", "X-XXXX-XXXX");
            public static PhoneNumberPattern spain => new("XXX XX XX XX", "XXXX XXXXX");
            public static PhoneNumberPattern sweden => new("XXX-XXX XX XX", "XXX-XXXXXXX");
            public static PhoneNumberPattern unitedKingdom => new("XXX XXXX XXXX", "XXXX XXX XXXX");
            public static PhoneNumberPattern unitedStatesPattern => new("(XXX) XXX-XXXX", "(XXX) XXX-XXXX");
            // ReSharper restore StringLiteralTypo
        }
    }

    public static class CountryCode
    {
        private static readonly Dictionary<string, string> Codes = new Dictionary<string, string>
        {
            { "1", "US/CA" },
            { "7", "RU/KZ" },
            { "20", "EG" },
            { "27", "ZA" },
            { "30", "GR" },
            { "31", "NL" },
            { "32", "BE" },
            { "33", "FR" },
            { "34", "ES" },
            { "36", "HU" },
            { "39", "IT" },
            { "40", "RO" },
            { "41", "CH" },
            { "43", "AT" },
            { "44", "GB/IM/JE/GG" },
            { "45", "DK" },
            { "46", "SE" },
            { "47", "NO/SJ/BV" },
            { "48", "PL" },
            { "49", "DE" },
            { "51", "PE" },
            { "52", "MX" },
            { "53", "CU" },
            { "54", "AR" },
            { "55", "BR" },
            { "56", "CL" },
            { "57", "CO" },
            { "58", "VE" },
            { "60", "MY" },
            { "61", "AU/CC/CX" },
            { "62", "ID" },
            { "63", "PH" },
            { "64", "NZ" },
            { "65", "SG" },
            { "66", "TH" },
            { "81", "JP" },
            { "82", "KR" },
            { "84", "VN" },
            { "86", "CN" },
            { "90", "TR" },
            { "91", "IN" },
            { "92", "PK" },
            { "93", "AF" },
            { "94", "LK" },
            { "95", "MM" },
            { "98", "IR" },
            { "211", "SS" },
            { "212", "MA/EH" },
            { "213", "DZ" },
            { "216", "TN" },
            { "218", "LY" },
            { "220", "GM" },
            { "221", "SN" },
            { "222", "MR" },
            { "223", "ML" },
            { "224", "GN" },
            { "225", "CI" },
            { "226", "BF" },
            { "227", "NE" },
            { "228", "TG" },
            { "229", "BJ" },
            { "230", "MU" },
            { "231", "LR" },
            { "232", "SL" },
            { "233", "GH" },
            { "234", "NG" },
            { "235", "TD" },
            { "236", "CF" },
            { "237", "CM" },
            { "238", "CV" },
            { "239", "ST" },
            { "240", "GQ" },
            { "241", "GA" },
            { "242", "CG" },
            { "243", "CD" },
            { "244", "AO" },
            { "245", "GW" },
            { "246", "IO" },
            { "248", "SC" },
            { "249", "SD" },
            { "250", "RW" },
            { "251", "ET" },
            { "252", "SO" },
            { "253", "DJ" },
            { "254", "KE" },
            { "255", "TZ" },
            { "256", "UG" },
            { "257", "BI" },
            { "258", "MZ" },
            { "260", "ZM" },
            { "261", "MG" },
            { "262", "RE/YT/TF" },
            { "263", "ZW" },
            { "264", "NA" },
            { "265", "MW" },
            { "266", "LS" },
            { "267", "BW" },
            { "268", "SZ" },
            { "269", "KM" },
            { "290", "SH/TA" },
            { "291", "ER" },
            { "297", "AW" },
            { "298", "FO" },
            { "299", "GL" },
            { "350", "GI" },
            { "351", "PT" },
            { "352", "LU" },
            { "353", "IE" },
            { "354", "IS" },
            { "355", "AL" },
            { "356", "MT" },
            { "357", "CY" },
            { "358", "FI/AX" },
            { "359", "BG" },
            { "370", "LT" },
            { "371", "LV" },
            { "372", "EE" },
            { "373", "MD" },
            { "374", "AM" },
            { "375", "BY" },
            { "376", "AD" },
            { "377", "MC" },
            { "378", "SM" },
            { "379", "VA" },
            { "380", "UA" },
            { "381", "RS/XK" },
            { "382", "ME" },
            { "383", "XK" },
            { "385", "HR" },
            { "386", "SI" },
            { "387", "BA" },
            { "389", "MK" },
            { "420", "CZ" },
            { "421", "SK" },
            { "423", "LI" },
            { "500", "FK" },
            { "501", "BZ" },
            { "502", "GT" },
            { "503", "SV" },
            { "504", "HN" },
            { "505", "NI" },
            { "506", "CR" },
            { "507", "PA" },
            { "508", "PM" },
            { "509", "HT" },
            { "590", "BL/MF/GP" },
            { "591", "BO" },
            { "592", "GY" },
            { "593", "EC" },
            { "594", "GF" },
            { "595", "PY" },
            { "596", "MQ" },
            { "597", "SR" },
            { "598", "UY" },
            { "599", "CW/BQ" },
            { "670", "TL" },
            { "672", "NF" },
            { "673", "BN" },
            { "674", "NR" },
            { "675", "PG" },
            { "676", "TO" },
            { "677", "SB" },
            { "678", "VU" },
            { "679", "FJ" },
            { "680", "PW" },
            { "681", "WS" },
            { "682", "CK" },
            { "683", "NU" },
            { "685", "WS" },
            { "686", "KI" },
            { "687", "NC" },
            { "688", "TV" },
            { "689", "PF" },
            { "690", "TK" },
            { "691", "FM" },
            { "692", "MH" },
            { "800", "001" }, // International Freephone Service
            { "808", "001" }, // International Shared Cost Service (ISCS)
            { "850", "KP" },
            { "852", "HK" },
            { "853", "MO" },
            { "855", "KH" },
            { "856", "LA" },
            { "870", "001" }, // Inmarsat "SNAC" service
            { "878", "001" }, // Universal Personal Telecommunications services
            { "880", "BD" },
            { "881", "001" }, // Global Mobile Satellite System (GMSS), shared code with 882
            { "882", "001" }, // Global Mobile Satellite System (GMSS), shared code with 881
            { "883", "001" }, // International Networks, shared code with 884
            { "886", "TW" },
            { "888", "001" }, // Global Customer Service, shared code with 877
            { "960", "MV" },
            { "961", "LB" },
            { "962", "JO" },
            { "963", "SY" },
            { "964", "IQ" },
            { "965", "KW" },
            { "966", "SA" },
            { "967", "YE" },
            { "968", "OM" },
            { "970", "PS" },
            { "971", "AE" },
            { "972", "IL" },
            { "973", "BH" },
            { "974", "QA" },
            { "975", "BT" },
            { "976", "MN" },
            { "977", "NP" },
            { "979", "001" }, // International Premium Rate Service (IPRS)
            { "992", "TJ" },
            { "993", "TM" },
            { "994", "AZ" },
            { "995", "GE" },
            { "996", "KG" },
            { "998", "UZ" }
        };

        /// <summary>
        /// Checks if the phone number starts with a valid country code and returns true if it does.
        /// Otherwise, returns false.
        /// </summary>
        /// <param name="phoneNumber"> The phone number to check. </param>
        /// <returns> True if the phone number starts with a valid country code. </returns>
        public static bool IsValid(string phoneNumber)
        {
            //check for + sign and remove it
            if (phoneNumber.StartsWith("+"))
                phoneNumber = phoneNumber.Substring(1);
            return Codes.Keys.Any(phoneNumber.StartsWith);
        }

        public static string GetCountryCode(string phoneNumber)
        {
            string code = string.Empty;

            // remove any non-digit characters from the phone number
            string cleanedNumber = new(phoneNumber.Where(char.IsDigit).ToArray());

            //remove + sign
            if (cleanedNumber.StartsWith("+")) cleanedNumber = cleanedNumber.Substring(1);

            //remove all leading 0
            while (cleanedNumber.StartsWith("0")) cleanedNumber = cleanedNumber.Substring(1);

            //trim
            cleanedNumber = cleanedNumber.Trim();

            //return if empty
            if (string.IsNullOrEmpty(cleanedNumber))
                return code;

            //check if the number starts with a valid country code this can have 1, 2 or 3 digits
            foreach (string key in Codes.Keys.Where(key => cleanedNumber.StartsWith(key)))
            {
                code = key;
                return code;
            }

            return code;
        }
    }
}
