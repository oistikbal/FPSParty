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
    /// Transforms a <see cref="System.TimeSpan"/> value as a string in a customizable format.
    /// <para/> The format string can contain the following placeholders: %d - Days, %h - Hours, %m - Minutes, %s - Seconds, %f - Fractional Seconds
    /// </summary>
    [CreateAssetMenu(fileName = "TimeSpan", menuName = "Doozy/Bindy/Transformer/TimeSpan", order = -950)]
    public class TimeSpanTransformer : ValueTransformer
    {
        public override string description =>
            "Transforms a TimeSpan value as a string in a customizable format.\n\n" +
            "The format string can contain the following placeholders: %d - Days, %h - Hours, %m - Minutes, %s - Seconds, %f - Fractional Seconds";
        
        protected override Type[] fromTypes => new[] { typeof(TimeSpan), typeof(string) };
        protected override Type[] toTypes => new[] { typeof(string) };

        [SerializeField] private string TimeSpanFormat = "%h\\:%m\\:%s";
        /// <summary> The format string to use for formatting the time span value. </summary>
        public string timeSpanFormat
        {
            get => TimeSpanFormat;
            set => TimeSpanFormat = value;
        }

        /// <summary>
        /// Transforms a TimeSpan value before it is displayed in a UI component.
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
                if (TimeSpan.TryParse(str, out TimeSpan timeSpanFromString))
                    source = timeSpanFromString;
            }

            if (source is TimeSpan timeSpanValue)
            {
                return timeSpanValue.ToString(timeSpanFormat);
            }

            return source.ToString();
        }
    }
}
