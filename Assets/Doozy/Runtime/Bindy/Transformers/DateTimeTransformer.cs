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
    /// Transforms a DateTime value as a string using a specified format string.
    /// </summary>
    [CreateAssetMenu(fileName = "DateTime", menuName = "Doozy/Bindy/Transformer/DateTime", order = -950)]
    public class DateTimeTransformer : ValueTransformer
    {
        public override string description =>
            "Transforms a DateTime value as a string using a specified format string.";

        protected override Type[] fromTypes => new[] { typeof(DateTime), typeof(string) };
        protected override Type[] toTypes => new[] { typeof(string) };

        /// <summary> The format string to use </summary>
        public enum DateTimeFormatType
        {
            /// <summary>
            /// Custom date and time format.
            /// </summary>
            Custom,

            /// <summary>
            /// Short date format. Example: 6/15/2009
            /// </summary>
            ShortDate,

            /// <summary>
            /// Long date format. Example: Monday, June 15, 2009
            /// </summary>
            LongDate,

            /// <summary>
            /// Full date and time format. Example: Monday, June 15, 2009 1:45:30 PM
            /// </summary>
            FullDateTime,

            /// <summary>
            /// General date and time format. Example: 6/15/2009 1:45:30 PM
            /// </summary>
            GeneralDate,

            /// <summary>
            /// Long time format. Example: 1:45:30 PM
            /// </summary>
            LongTime,

            /// <summary>
            /// Short time format. Example: 1:45 PM
            /// </summary>
            ShortTime,

            /// <summary>
            /// Month, day, and year format. Example: June 15, 2009
            /// </summary>
            MonthDayYear,

            /// <summary>
            /// Month and year format. Example: June 2009
            /// </summary>
            MonthYear,

            /// <summary>
            /// Month and day format. Example: June 15
            /// </summary>
            MonthDay,

            /// <summary>
            /// Day, month, and year format. Example: 15 June 2009
            /// </summary>
            DayMonthYear,

            /// <summary>
            /// Year, month, and day format. Example: 2009-06-15
            /// </summary>
            YearMonthDay,

            /// <summary>
            /// Weekday, month, day, and year format. Example: Monday, June 15, 2009
            /// </summary>
            WeekdayMonthDayYear,

            /// <summary>
            /// Weekday and month/day format. Example: Monday, June 15
            /// </summary>
            WeekdayMonthDay,

            /// <summary>
            /// Weekday abbreviation, month, day, and year format. Example: Mon, June 15, 2009
            /// </summary>
            WeekdayAbbrMonthDayYear,

            /// <summary>
            /// Weekday abbreviation and month/day format. Example: Mon, June 15
            /// </summary>
            WeekdayAbbrMonthDay,

            /// <summary>
            /// Hour and minute in 24-hour clock format. Example: 13:45
            /// </summary>
            HourMinute24,

            /// <summary>
            /// Hour, minute, and second in 24-hour clock format. Example: 13:45:30
            /// </summary>
            HourMinuteSecond24,

            /// <summary>
            /// Hour and minute in 12-hour clock format. Example: 1:45 PM
            /// </summary>
            HourMinute12,

            /// <summary>
            /// Hour, minute, and second in 12-hour clock format. Example: 1:45:30 PM
            /// </summary>
            HourMinuteSecond12
        }


        public enum CultureType
        {
            /// <summary>
            /// The culture of the current thread will be used to format the DateTime value.
            /// </summary>
            Current,

            /// <summary>
            /// The invariant culture will be used to format the DateTime value.
            /// </summary>
            Invariant,

            /// <summary>
            /// The culture specified by cultureName will be used to format the DateTime value.
            /// </summary>
            Specific
        }

        [SerializeField] private DateTimeFormatType FormatType = DateTimeFormatType.Custom;
        /// <summary>
        /// The type of format to use.
        /// </summary>
        public DateTimeFormatType formatType
        {
            get => FormatType;
            set => FormatType = value;
        }

        [SerializeField] private string CustomDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// The custom format string to use when formatType is set to Custom.
        /// </summary>
        public string customDateTimeFormat
        {
            get => CustomDateTimeFormat;
            set => CustomDateTimeFormat = value;
        }

        [SerializeField] private CultureType Culture = CultureType.Invariant;
        /// <summary>
        /// The culture to use when formatting the DateTime value.
        /// </summary>
        public CultureType culture
        {
            get => Culture;
            set => Culture = value;
        }

        [SerializeField] private string CultureName = "en-US";
        /// <summary>
        /// The culture name to use when formatting the DateTime value.
        /// </summary>
        public string cultureName
        {
            get => CultureName;
            set => CultureName = value;
        }

        /// <summary>
        /// Transforms a DateTime value as a string using a specified format string (dateTimeFormat).
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (source.GetType() != typeof(DateTime)) return source;
            if (!enabled) return source;

            var dateTimeValue = (DateTime)source;
            CultureInfo cultureInfo = culture == CultureType.Specific ? new CultureInfo(cultureName) : GetCultureInfoFromType(culture);
            string formatString = GetFormatString(formatType);
            return dateTimeValue.ToString(formatString, cultureInfo);
        }

        /// <summary>
        /// Gets the CultureInfo from the specified CultureType.
        /// </summary>
        /// <param name="cultureType"> The CultureType to get the CultureInfo for. </param>
        /// <returns> The CultureInfo for the specified CultureType. </returns>
        /// <exception cref="ArgumentException"> Thrown when the specified CultureType is not supported. </exception>
        private CultureInfo GetCultureInfoFromType(CultureType cultureType)
        {
            switch (cultureType)
            {
                case CultureType.Invariant: return CultureInfo.InvariantCulture;
                case CultureType.Current: return CultureInfo.CurrentCulture;
                case CultureType.Specific: return new CultureInfo(cultureName);
                default: throw new ArgumentException($"Unsupported culture type: {cultureType}");
            }
        }

        /// <summary>
        /// Gets the format string for the specified DateTimeFormatType.
        /// </summary>
        /// <param name="formatType"> The DateTimeFormatType to get the format string for. </param>
        /// <returns> The format string for the specified DateTimeFormatType. </returns>
        private static string GetFormatString(DateTimeFormatType formatType)
        {
            switch (formatType)
            {
                case DateTimeFormatType.Custom: return "yyyy-MM-dd HH:mm:ss";
                case DateTimeFormatType.ShortDate: return "d";
                case DateTimeFormatType.LongDate: return "D";
                case DateTimeFormatType.FullDateTime: return "F";
                case DateTimeFormatType.GeneralDate: return "g";
                case DateTimeFormatType.LongTime: return "T";
                case DateTimeFormatType.ShortTime: return "t";
                case DateTimeFormatType.MonthDayYear: return "MM/dd/yyyy";
                case DateTimeFormatType.MonthYear: return "MM/yyyy";
                case DateTimeFormatType.MonthDay: return "MM/dd";
                case DateTimeFormatType.DayMonthYear: return "dd/MM/yyyy";
                case DateTimeFormatType.YearMonthDay: return "yyyy/MM/dd";
                case DateTimeFormatType.WeekdayMonthDayYear: return "dddd, MMMM dd, yyyy";
                case DateTimeFormatType.WeekdayMonthDay: return "dddd, MMMM dd";
                case DateTimeFormatType.WeekdayAbbrMonthDayYear: return "ddd, MMMM dd, yyyy";
                case DateTimeFormatType.WeekdayAbbrMonthDay: return "ddd, MMMM dd";
                case DateTimeFormatType.HourMinute24: return "HH:mm";
                case DateTimeFormatType.HourMinuteSecond24: return "HH:mm:ss";
                case DateTimeFormatType.HourMinute12: return "h:mm tt";
                case DateTimeFormatType.HourMinuteSecond12: return "h:mm:ss tt";
                default: return "yyyy-MM-dd HH:mm:ss";
            }
        }
    }
}
