// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable ReplaceSubstringWithRangeIndexer
// ReSharper disable UseNegatedPatternInIsExpression

namespace Doozy.Runtime.Bindy.Transformers
{
    /// <summary>
    /// Transforms a string based on the selected option.
    /// </summary>
    [CreateAssetMenu(fileName = "Text", menuName = "Doozy/Bindy/Transformer/Text", order = -950)]
    public class TextTransformer : ValueTransformer
    {
        public override string description => 
            "Transforms a string based on the selected option.";
        
        protected override Type[] fromTypes => new[] { typeof(string) };
        protected override Type[] toTypes => new[] { typeof(string) };

        private const char k_SnakeCaseSeparator = '_';                                    //used to avoid garbage collection
        private const char k_KebabCaseSeparator = '-';                                    //used to avoid garbage collection
        private const char k_SpaceSeparator = ' ';                                        //used to avoid garbage collection
        private static readonly char[] SpaceSeparators = { ' ', '\t', '\r', '\n', '\v' }; //used to avoid garbage collection

        /// <summary> Specifies the different text transformation modes supported by the StringTransformer class. </summary>
        public enum TransformMode
        {
            None,
            LowerCaseWords,
            FirstWordCapitalized,
            CapitalizedWords,
            CamelCase,
            SnakeCase,
            KebabCase,
            PascalCase,
            ScreamingSnakeCase,
            CapitalizedSnakeCase,
            Capitalize,
            ToLowerCase,
            ToUpperCase,
            ToTitleCase,
            ToSentenceCase,
            InvertCase
        }

        private StringBuilder m_Builder;
        private StringBuilder builder => m_Builder ?? (m_Builder = new StringBuilder());

        [SerializeField] private TransformMode Mode = TransformMode.LowerCaseWords;
        /// <summary>
        /// The mode in which the transformer will operate.
        /// </summary>
        public TransformMode mode
        {
            get => Mode;
            set => Mode = value;
        }

        [SerializeField] private bool Trim;
        /// <summary> Whether to trim the string. </summary>
        public bool trim
        {
            get => Trim;
            set => Trim = value;
        }

        [SerializeField] private bool RemoveExtraSpaces;
        /// <summary> Whether to remove extra spaces from the string. </summary>
        public bool removeExtraSpaces
        {
            get => RemoveExtraSpaces;
            set => RemoveExtraSpaces = value;
        }

        [SerializeField] private bool RemoveDiacritics;
        /// <summary> Whether to remove diacritics from the string. </summary>
        public bool removeDiacritics
        {
            get => RemoveDiacritics;
            set => RemoveDiacritics = value;
        }

        [SerializeField] private bool RemoveNonAlphanumeric;
        /// <summary> Whether to remove non-alphanumeric characters from the string. </summary>
        public bool removeNonAlphanumeric
        {
            get => RemoveNonAlphanumeric;
            set => RemoveNonAlphanumeric = value;
        }

        [SerializeField] private bool RemoveCharacters;
        /// <summary> Whether to remove characters from the string. </summary>
        public bool removeCharacters
        {
            get => RemoveCharacters;
            set => RemoveCharacters = value;
        }

        [SerializeField] private string CharactersToRemove = string.Empty;
        /// <summary> The characters to remove from the string. </summary>
        public string charactersToRemove
        {
            get => CharactersToRemove;
            set => CharactersToRemove = value;
        }

        [SerializeField] private bool Reverse;
        /// <summary> Whether to reverse the string. </summary>
        public bool reverse
        {
            get => Reverse;
            set => Reverse = value;
        }

        [SerializeField] private bool RemoveTabs;
        /// <summary> Whether to remove tabs from the string. </summary>
        public bool removeTabs
        {
            get => RemoveTabs;
            set => RemoveTabs = value;
        }

        [SerializeField] private bool RemoveNewLines;
        /// <summary> Whether to remove new lines from the string. </summary>
        public bool removeNewLines
        {
            get => RemoveNewLines;
            set => RemoveNewLines = value;
        }

        [SerializeField] private bool RemoveLineBreaks;
        /// <summary> Whether to remove line breaks from the string. </summary>
        public bool removeLineBreaks
        {
            get => RemoveLineBreaks;
            set => RemoveLineBreaks = value;
        }

        [SerializeField] private bool RemoveVerticalTabs;
        /// <summary> Whether to remove vertical tabs from the string. </summary>
        public bool removeVerticalTabs
        {
            get => RemoveVerticalTabs;
            set => RemoveVerticalTabs = value;
        }

        [SerializeField] private bool Truncate;
        /// <summary> Whether to truncate the string to the maximum length. </summary>
        public bool truncate
        {
            get => Truncate;
            set => Truncate = value;
        }

        [SerializeField] private int MaxLength = 10;
        /// <summary> The maximum length of the truncated string. </summary>
        public int maxLength
        {
            get => MaxLength;
            set => MaxLength = value;
        }

        [SerializeField] private string TruncationIndicator = "...";
        /// <summary> The string to append to the end of the truncated string. </summary>
        public string truncationIndicator
        {
            get => TruncationIndicator;
            set => TruncationIndicator = value;
        }

        /// <summary>
        /// Transforms a string based on the selected options.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (!(source is string str)) return source;
            if (!enabled) return source;

            str = GetTransform(str); // Transform the string based on the selected transform mode

            if (removeDiacritics) str = RemoveDiacriticsFromString(str);           // Remove diacritics
            if (removeNonAlphanumeric) str = RemoveNonAlphanumericFromString(str); // Remove non-alphanumeric characters

            if (removeCharacters && !string.IsNullOrEmpty(charactersToRemove))
                str = RemoveCharactersFromString(str, charactersToRemove); // Remove characters

            if (trim) str = str.Trim();                                    // Trim
            if (removeExtraSpaces) str = RemoveExtraSpacesFromString(str); // Remove extra spaces
            if (removeTabs) str = str.Replace("\t", "");                   // Remove tabs
            if (removeNewLines) str = str.Replace("\n", "");               // Remove new lines
            if (removeLineBreaks) str = str.Replace("\r", "");             // Remove line breaks
            if (removeVerticalTabs) str = str.Replace("\v", "");           // Remove vertical tabs

            if (reverse) // Reverse
            {
                char[] arr = str.ToCharArray();
                Array.Reverse(arr);
                str = new string(arr);
            }

            // ReSharper disable once InvertIf
            if (truncate) // Truncate
            {
                maxLength = Mathf.Max(0, maxLength);

                if (maxLength == 0)
                    return string.Empty;

                if (str.Length > maxLength)
                    str = str.Substring(0, maxLength) + truncationIndicator;
            }

            return str;
        }

        /// <summary>
        /// Gets the transform based on the selected option.
        /// </summary>
        /// <param name="str"> The string to transform </param>
        /// <returns> The transformed string </returns>
        private string GetTransform(string str)
        {
            switch (mode)
            {
                case TransformMode.LowerCaseWords: return LowerCaseWords(str);
                case TransformMode.FirstWordCapitalized: return FirstWordCapitalized(str);
                case TransformMode.CapitalizedWords: return CapitalizedWords(str);
                case TransformMode.CamelCase: return CamelCase(str);
                case TransformMode.SnakeCase: return SnakeCase(str);
                case TransformMode.KebabCase: return KebabCase(str);
                case TransformMode.PascalCase: return PascalCase(str);
                case TransformMode.ScreamingSnakeCase: return ScreamingSnakeCase(str);
                case TransformMode.CapitalizedSnakeCase: return CapitalizedSnakeCase(str);
                case TransformMode.Capitalize: return Capitalize(str);
                case TransformMode.ToLowerCase: return ToLowerCase(str);
                case TransformMode.ToUpperCase: return ToUpperCase(str);
                case TransformMode.ToTitleCase: return ToTitleCase(str);
                case TransformMode.ToSentenceCase: return ToSentenceCase(str);
                case TransformMode.InvertCase: return InvertCase(str);
                case TransformMode.None:
                default: return str;
            }
        }

        /// <summary>
        /// Removes diacritics (accent marks) from a string.
        /// </summary>
        /// <param name="str">The string to remove diacritics from.</param>
        /// <returns>The string with diacritics removed.</returns>
        private static string RemoveDiacriticsFromString(string str)
        {
            string normalized = str.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < normalized.Length; i++)
            {
                char c = normalized[i];
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(c);
                }
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Removes all non-alphanumeric characters from a string.
        /// </summary>
        /// <param name="str">The string to remove non-alphanumeric characters from.</param>
        /// <returns>The string with non-alphanumeric characters removed.</returns>
        private static string RemoveNonAlphanumericFromString(string str)
        {
            const string pattern = @"[^a-zA-Z0-9]";
            return Regex.Replace(str, pattern, string.Empty);
        }

        /// <summary>
        /// Removes all specified characters from a string.
        /// </summary>
        /// <param name="str">The string to remove characters from.</param>
        /// <param name="charactersToRemove">The characters to remove from the string.</param>
        /// <returns>The string with specified characters removed.</returns>
        private static string RemoveCharactersFromString(string str, string charactersToRemove)
        {
            string pattern = $"[{Regex.Escape(charactersToRemove)}]";
            return Regex.Replace(str, pattern, string.Empty);
        }

        /// <summary>
        /// Removes all extra spaces from a string.
        /// </summary>
        /// <param name="str">The string to remove extra spaces from.</param>
        /// <returns>The string with extra spaces removed.</returns>
        private static string RemoveExtraSpacesFromString(string str) =>
            str.RemoveWhitespaces();

        /// <summary>
        /// Transforms a string to lowercase words, where the first letter of each word is lowercase.
        /// </summary>
        /// <param name="str">The string to transform.</param>
        /// <returns>The transformed string.</returns>
        private string LowerCaseWords(string str)
        {
            string[] words = str.Split(SpaceSeparators, StringSplitOptions.RemoveEmptyEntries);
            builder.Clear();
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (word.Length > 1)
                {
                    builder.Append(char.ToLower(word[0]));
                    builder.Append(word.Substring(1).ToLower());
                }
                else
                {
                    builder.Append(word.ToLower());
                }

                if (i < words.Length - 1)
                {
                    builder.Append(k_SpaceSeparator);
                }
            }

            return builder.ToString();
        }


        /// <summary>
        /// Capitalizes the first letter of the first word in a string.
        /// </summary>
        /// <param name="str">The string to transform.</param>
        /// <returns>The transformed string.</returns>
        private string FirstWordCapitalized(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;

            builder.Clear();
            string[] words = str.Split(SpaceSeparators, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0) return str;

            builder.Append(char.ToUpper(words[0][0]));
            builder.Append(words[0].Substring(1));
            for (int i = 1; i < words.Length; i++)
            {
                builder.Append(k_SpaceSeparator);
                builder.Append(words[i]);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Transforms a string to Capitalized Words, where the first letter of each word is capitalized.
        /// </summary>
        /// <param name="str">The string to transform.</param>
        /// <returns>The transformed string.</returns>
        private string CapitalizedWords(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;

            builder.Clear();
            string[] words = str.Split(SpaceSeparators, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (string.IsNullOrEmpty(word)) continue;

                builder.Append(char.ToUpper(word[0]));
                builder.Append(word.Substring(1));

                if (i < words.Length - 1)
                {
                    builder.Append(k_SpaceSeparator);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Transforms a string to CamelCase, where the first letter of each word after the first is capitalized and there are no spaces.
        /// </summary>
        /// <param name="str"> The string to transform </param>
        /// <returns> The transformed string </returns>
        private string CamelCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            string[] words = str.Split(SpaceSeparators, StringSplitOptions.RemoveEmptyEntries);
            builder.Clear();
            for (int i = 0; i < words.Length; i++)
            {
                if (string.IsNullOrEmpty(words[i])) continue;
                if (i == 0)
                {
                    builder.Append(words[i].ToLower());
                }
                else
                {
                    builder.Append(words[i][0].ToString().ToUpper());
                    builder.Append(words[i].Substring(1).ToLower());
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Transforms a string to snake_case, where words are separated by underscores and all letters are lowercase.
        /// </summary>
        /// <param name="str">The string to transform.</param>
        /// <returns>The transformed string.</returns>
        private string SnakeCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;

            string[] words = str.Split(SpaceSeparators, StringSplitOptions.RemoveEmptyEntries);
            builder.Clear();

            for (int i = 0; i < words.Length; i++)
            {
                if (string.IsNullOrEmpty(words[i])) continue;

                if (i > 0)
                    builder.Append(k_SnakeCaseSeparator);

                builder.Append(words[i].ToLower());
            }

            return builder.ToString();
        }

        /// <summary>
        /// Transforms a string to kebab-case, where words are separated by hyphens and all letters are lowercase.
        /// </summary>
        /// <param name="str"> The string to transform </param>
        /// <returns> The transformed string </returns>
        private string KebabCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            string[] words = str.Split(SpaceSeparators, StringSplitOptions.RemoveEmptyEntries);
            builder.Clear();
            for (int i = 0; i < words.Length; i++)
            {
                if (string.IsNullOrEmpty(words[i])) continue;
                builder.Append(words[i].ToLower());
                if (i < words.Length - 1)
                {
                    builder.Append(k_KebabCaseSeparator);
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Transforms a string to PascalCase, where the first letter of each word is capitalized and there are no spaces.
        /// </summary>
        /// <param name="str"> The string to transform </param>
        /// <returns> The transformed string </returns>
        private string PascalCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            string[] words = str.Split(SpaceSeparators, StringSplitOptions.RemoveEmptyEntries);
            builder.Clear();
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < words.Length; i++)
            {
                if (string.IsNullOrEmpty(words[i])) continue;
                builder.Append(words[i][0].ToString().ToUpper());
                builder.Append(words[i].Substring(1));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Transforms a string to SCREAMING_SNAKE_CASE, where words are separated by underscores and all letters are uppercase.
        /// </summary>
        /// <param name="str"> The string to transform </param>
        /// <returns> The transformed string </returns>
        private string ScreamingSnakeCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            string[] words = str.Split(SpaceSeparators, StringSplitOptions.RemoveEmptyEntries);
            builder.Clear();
            for (int i = 0; i < words.Length; i++)
            {
                if (string.IsNullOrEmpty(words[i])) continue;
                builder.Append(words[i].ToUpper());
                if (i < words.Length - 1)
                {
                    builder.Append(k_SnakeCaseSeparator);
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Converts a string to Capitalized_Snake_Case.
        /// </summary>
        /// <param name="str"> The string to transform </param>
        /// <returns> The transformed string </returns>
        private string CapitalizedSnakeCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            string[] words = str.Split(SpaceSeparators, StringSplitOptions.RemoveEmptyEntries);
            builder.Clear();
            for (int i = 0; i < words.Length; i++)
            {
                if (string.IsNullOrEmpty(words[i])) continue;
                builder.Append(words[i].Substring(0, 1).ToUpper());
                if (words[i].Length > 1)
                {
                    builder.Append(words[i].Substring(1).ToLower());
                }
                if (i < words.Length - 1)
                {
                    builder.Append(k_SnakeCaseSeparator);
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Capitalizes the first letter of every word in a string.
        /// </summary>
        /// <param name="str"> The string to transform </param>
        /// <returns> The transformed string </returns>
        private string Capitalize(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            string[] words = str.Split(SpaceSeparators, StringSplitOptions.RemoveEmptyEntries);
            builder.Clear();
            for (int i = 0; i < words.Length; i++)
            {
                if (string.IsNullOrEmpty(words[i])) continue;
                builder.Append(words[i].Substring(0, 1).ToUpper());
                if (words[i].Length > 1)
                {
                    builder.Append(words[i].Substring(1));
                }
                if (i < words.Length - 1)
                {
                    builder.Append(k_SpaceSeparator);
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Converts a string to lowercase.
        /// </summary>
        /// <param name="str"> The string to transform </param>
        /// <returns> The transformed string </returns>
        private static string ToLowerCase(string str) =>
            string.IsNullOrEmpty(str) ? str : str.ToLower();

        /// <summary>
        /// Converts a string to uppercase.
        /// </summary>
        /// <param name="str"> The string to transform </param>
        /// <returns> The transformed string </returns>
        private static string ToUpperCase(string str) =>
            string.IsNullOrEmpty(str) ? str : str.ToUpper();

        /// <summary>
        /// Transforms a string to Title Case, where the first letter of each word is capitalized and all other letters are lowercase.
        /// </summary>
        /// <param name="str"> The string to transform </param>
        /// <returns> The transformed string </returns>
        private string ToTitleCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;

            builder.Clear();
            string[] words = str.Split(SpaceSeparators, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                if (string.IsNullOrEmpty(words[i])) continue;
                builder.Append(words[i][0].ToString().ToUpper());
                builder.Append(words[i].Substring(1).ToLower());
                if (i < words.Length - 1)
                {
                    builder.Append(k_SpaceSeparator);
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Transforms a string to Sentence Case, where the first letter of the first word is capitalized and all other letters are lowercase.
        /// </summary>
        /// <param name="str"> The string to transform </param>
        /// <returns> The transformed string </returns>
        private string ToSentenceCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;

            builder.Clear();
            
            string[] words = str.Split(SpaceSeparators, StringSplitOptions.RemoveEmptyEntries);
            
            if (words.Length <= 0)
                return builder.ToString();
            
            builder.Append(words[0].Substring(0, 1).ToUpper());
            builder.Append(words[0].Substring(1).ToLower());
            for (int i = 1; i < words.Length; i++)
            {
                if (string.IsNullOrEmpty(words[i])) continue;
                builder.Append(k_SpaceSeparator);
                builder.Append(words[i].ToLower());
            }
            return builder.ToString();
        }

        /// <summary>
        /// Inverts the case of every letter in a string.
        /// </summary>
        /// <param name="str"> The string to transform </param>
        /// <returns> The transformed string </returns>
        private string InvertCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;

            builder.Clear();
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (char.IsLower(c))
                {
                    builder.Append(char.ToUpper(c));
                }
                else if (char.IsUpper(c))
                {
                    builder.Append(char.ToLower(c));
                }
                else
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }
    }
}
