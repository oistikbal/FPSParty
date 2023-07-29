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
    /// Transforms a time value in seconds to a formatted string.
    /// <para/> For example, 120 seconds will be formatted to 02:00.00.
    /// Or 60 seconds will be formatted to 01:00.00.
    /// </summary>
    [CreateAssetMenu(fileName = "Time", menuName = "Doozy/Bindy/Transformer/Time", order = -950)]
    public class TimeTransformer : ValueTransformer
    {
        public override string description =>
            "Transforms a time value in seconds to a formatted string.\n\n" +
            "For example, 120 seconds will be formatted to 02:00.00.\n\n" +
            "Or 60 seconds will be formatted to 01:00.00.";   
        
        protected override Type[] fromTypes => new[] { typeof(float), typeof(double), typeof(DateTime), typeof(TimeSpan), typeof(string) };
        protected override Type[] toTypes => new[] { typeof(string) };
        
        [SerializeField] private string TimeFormat = @"mm\:ss\.ff";
        /// <summary> The format string to use for formatting the time value. </summary>
        public string timeFormat
        {
            get => TimeFormat;
            set => TimeFormat = value;
        }

        /// <summary>
        /// Transforms a time value before it is displayed in a UI component.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (!enabled) return source;

            if (source is string str)
            {
                if (float.TryParse(str, out float floatFromString))
                    source = floatFromString;
                else if (double.TryParse(str, out double doubleFromString))
                    source = doubleFromString;
            }

            switch (source)
            {
                case float floatValue:
                {
                    var floatToTimeSpan = TimeSpan.FromSeconds(floatValue);
                    return floatToTimeSpan.ToString(timeFormat);
                }
                case double doubleValue:
                {
                    var doubleToTimeSpan = TimeSpan.FromSeconds(doubleValue);
                    return doubleToTimeSpan.ToString(timeFormat);
                }
                case DateTime dateTime:
                    return dateTime.ToString(timeFormat);
                case TimeSpan timeSpan:
                    return timeSpan.ToString(timeFormat);
                default:
                    return source.ToString();
            }
        }
    }
}
