// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
using UnityEngine.Serialization;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UseNegatedPatternInIsExpression

namespace Doozy.Runtime.Bindy.Transformers
{
    /// <summary>
    /// Transforms a TimeSpan value by returning its duration in hours, minutes, and seconds (e.g. "1h 30m 10s").
    /// </summary>
    [CreateAssetMenu(fileName = "TimeSpan to Duration", menuName = "Doozy/Bindy/Transformer/TimeSpan to Duration", order = -950)]
    public class TimeSpanToDurationTransformer : ValueTransformer
    {
        public override string description =>
            "Transforms a TimeSpan value by returning its duration in hours, minutes, and seconds (e.g. \"1h 30m 10s\").";

        protected override Type[] fromTypes => new[] { typeof(TimeSpan) };
        protected override Type[] toTypes => new[] { typeof(string) };

        [FormerlySerializedAs("format")]
        [SerializeField] private string DurationFormat = "{0}h {1}m {2}s";
        /// <summary> The format string to use for the duration. </summary>
        public string durationFormat
        {
            get => DurationFormat;
            set => DurationFormat = value;
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
            if (!(source is TimeSpan timeSpan)) return source;
            return
                enabled
                    ? string.Format(durationFormat, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds)
                    : source;

        }
    }
}
