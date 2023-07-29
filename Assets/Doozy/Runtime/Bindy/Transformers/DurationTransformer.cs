// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
namespace Doozy.Runtime.Bindy.Transformers
{
    /// <summary>
    /// Transforms a duration value (in seconds) as a human-readable string, with options for different units (e.g. minutes, hours, days).
    /// The transformer supports an optional format string in the options, which allows for customization of the output.
    /// The default format is "h'h 'm'm 's's'", which produces output in the format "Xh Ym Zs", where X is the number of hours, Y is the number of minutes, and Z is the number of seconds.
    /// <para/> The format string can include the following characters: d - days | h - hours | m - minutes | s - seconds
    /// <para/> The format string can also include the following escape sequences (padded with a leading zero if necessary): hh - hours | mm - minutes | ss - seconds
    /// <para/> For example, the format string "hh:mm:ss" will produce output in the format "XX:YY:ZZ", where XX is the number of hours, YY is the number of minutes, and ZZ is the number of seconds.
    /// </summary>
    [CreateAssetMenu(fileName = "Duration", menuName = "Doozy/Bindy/Transformer/Duration", order = -950)]
    public class DurationTransformer : ValueTransformer
    {
        public override string description =>
            "Transforms a duration value (in seconds) as a human-readable string, with options for different units (e.g. minutes, hours, days).\n\n" +
            "The transformer supports an optional format string in the options, which allows for customization of the output.\n\n" +
            "The default format is \"h'h 'm'm 's's'\", which produces output in the format \"Xh Ym Zs\", where X is the number of hours, Y is the number of minutes, and Z is the number of seconds.\n\n" +
            "The format string can include the following characters: d - days | h - hours | m - minutes | s - seconds\n\n" +
            "The format string can also include the following escape sequences (padded with a leading zero if necessary): hh - hours | mm - minutes | ss - seconds\n\n" +
            "For example, the format string \"hh:mm:ss\" will produce output in the format \"XX:YY:ZZ\", where XX is the number of hours, YY is the number of minutes, and ZZ is the number of seconds.";
        
        protected override Type[] fromTypes => new[] { typeof(float) };
        protected override Type[] toTypes => new[] { typeof(string) };

        /// <summary>
        /// The format string to use for formatting the duration value. The format string can include the following characters: d - days | h - hours | m - minutes | s - seconds
        /// The format string can also include the following escape sequences (padded with a leading zero if necessary): hh - hours | mm - minutes | ss - seconds
        /// </summary>
        [SerializeField] private string DurationFormat = "h'h 'm'm 's's'";
        public string durationFormat
        {
            get => DurationFormat;
            set => DurationFormat = value;
        }

        /// <summary>
        /// Transforms a duration value as a human-readable string (e.g. "1h 30m 15s")
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (!enabled) return source;

            if (source is not float duration)
                return source;

            var timeSpan = TimeSpan.FromSeconds(duration);

            if (durationFormat.Contains("d"))
            {
                // if format includes days, use TimeSpan to format the duration
                durationFormat = durationFormat.Replace("d", @"d\d\ hh\:mm\:ss");
                return timeSpan.ToString(durationFormat);
            }

            // otherwise, calculate the duration and format manually
            int days = (int)(duration / 86400);
            int hours = (int)(duration / 3600) % 24;
            int minutes = (int)(duration / 60) % 60;
            int seconds = (int)duration % 60;

            string result = "";
            if (days > 0)
                result += $"{days}d ";
            if (hours > 0)
                result += $"{hours}h ";
            if (minutes > 0)
                result += $"{minutes}m ";
            if (seconds > 0)
                result += $"{seconds}s ";

            return result.TrimEnd();
        }
    }
}
       
