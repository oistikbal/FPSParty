// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
// ReSharper disable UnusedMember.Global
namespace Doozy.Runtime.Bindy.Transformers
{
    /// <summary>
    /// Transforms a DateTime value by returning how long ago the time was (e.g. "just now", "5 minutes ago", "yesterday", etc.).
    /// </summary>
    [CreateAssetMenu(fileName = "DateTime To Time Ago", menuName = "Doozy/Bindy/Transformer/DateTime to Time Ago", order = -950)]
    public class DateTimeToTimeAgoTransformer : ValueTransformer
    {
        public override string description =>
            "Transforms a DateTime value by returning how long ago the time was (e.g. \"just now\", \"5 minutes ago\", \"yesterday\", etc.).";

        protected override Type[] fromTypes => new[] { typeof(DateTime) };
        protected override Type[] toTypes => new[] { typeof(string) };

        [SerializeField] private string JustNowText = "just now";
        /// <summary> The text to use when the time difference is less than one minute. </summary>
        public string justNowText
        {
            get => JustNowText;
            set => JustNowText = value;
        }

        [SerializeField] private string PastTextFormat = "{0} ago";
        /// <summary> The format string to use for time differences in the past. </summary>
        public string pastTextFormat
        {
            get => PastTextFormat;
            set => PastTextFormat = value;
        }

        [SerializeField] private string FutureTextFormat = "in {0}";
        /// <summary> The format string to use for time differences in the future. </summary>
        public string futureTextFormat
        {
            get => FutureTextFormat;
            set => FutureTextFormat = value;
        }

        [SerializeField] private string SecondTextSingular = "second";
        /// <summary> The singular form of the word 'second'. </summary>
        public string secondTextSingular
        {
            get => SecondTextSingular;
            set => SecondTextSingular = value;
        }

        [SerializeField] private string SecondTextPlural = "seconds";
        /// <summary> The plural form of the word 'second'. </summary>
        public string secondTextPlural
        {
            get => SecondTextPlural;
            set => SecondTextPlural = value;
        }

        [SerializeField] private string MinuteTextSingular = "minute";
        /// <summary> The singular form of the word 'minute'. </summary>
        public string minuteTextSingular
        {
            get => MinuteTextSingular;
            set => MinuteTextSingular = value;
        }

        [SerializeField] private string MinuteTextPlural = "minutes";
        /// <summary> The plural form of the word 'minute'. </summary>
        public string minuteTextPlural
        {
            get => MinuteTextPlural;
            set => MinuteTextPlural = value;
        }

        [SerializeField] private string HourTextSingular = "hour";
        /// <summary> The singular form of the word 'hour'. </summary>
        public string hourTextSingular
        {
            get => HourTextSingular;
            set => HourTextSingular = value;
        }

        [SerializeField] private string HourTextPlural = "hours";
        /// <summary> The plural form of the word 'hour'. </summary>
        public string hourTextPlural
        {
            get => HourTextPlural;
            set => HourTextPlural = value;
        }

        [SerializeField] private string DayTextSingular = "day";
        /// <summary> The singular form of the word 'day'. </summary>
        public string dayTextSingular
        {
            get => DayTextSingular;
            set => DayTextSingular = value;
        }

        [SerializeField] private string DayTextPlural = "days";
        /// <summary> The plural form of the word 'day'. </summary>
        public string dayTextPlural
        {
            get => DayTextPlural;
            set => DayTextPlural = value;
        }

        [SerializeField] private string WeekTextSingular = "week";
        /// <summary> The singular form of the word 'week'. </summary>
        public string weekTextSingular
        {
            get => WeekTextSingular;
            set => WeekTextSingular = value;
        }

        [SerializeField] private string WeekTextPlural = "weeks";
        /// <summary> The plural form of the word 'week'. </summary>
        public string weekTextPlural
        {
            get => WeekTextPlural;
            set => WeekTextPlural = value;
        }

        [SerializeField] private string MonthTextSingular = "month";
        /// <summary> The singular form of the word 'month'. </summary>
        public string monthTextSingular
        {
            get => MonthTextSingular;
            set => MonthTextSingular = value;
        }

        [SerializeField] private string MonthTextPlural = "months";
        /// <summary> The plural form of the word 'month'. </summary>
        public string monthTextPlural
        {
            get => MonthTextPlural;
            set => MonthTextPlural = value;
        }

        [SerializeField] private string YearTextSingular = "year";
        /// <summary> The singular form of the word 'year'. </summary>
        public string yearTextSingular
        {
            get => YearTextSingular;
            set => YearTextSingular = value;
        }

        [SerializeField] private string YearTextPlural = "years";
        /// <summary> The plural form of the word 'year'. </summary>
        public string yearTextPlural
        {
            get => YearTextPlural;
            set => YearTextPlural = value;
        }

        [SerializeField] private string YesterdayText = "yesterday";
        /// <summary> The text to use when the time difference is one day. </summary>
        public string yesterdayText
        {
            get => YesterdayText;
            set => YesterdayText = value;
        }

        [SerializeField] private string TomorrowText = "tomorrow";
        /// <summary> The text to use when the time difference is one day. </summary>
        public string tomorrowText
        {
            get => TomorrowText;
            set => TomorrowText = value;
        }

        [SerializeField] private string LastWeekText = "last week";
        /// <summary> The text to use when the time difference is one week. </summary>
        public string lastWeekText
        {
            get => LastWeekText;
            set => LastWeekText = value;
        }

        [SerializeField] private string NextWeekText = "next week";
        /// <summary> The text to use when the time difference is one week. </summary>
        public string nextWeekText
        {
            get => NextWeekText;
            set => NextWeekText = value;
        }

        [SerializeField] private string LastMonthText = "last month";
        /// <summary> The text to use when the time difference is one month. </summary>
        public string lastMonthText
        {
            get => LastMonthText;
            set => LastMonthText = value;
        }

        [SerializeField] private string NextMonthText = "next month";
        /// <summary> The text to use when the time difference is one month. </summary>
        public string nextMonthText
        {
            get => NextMonthText;
            set => NextMonthText = value;
        }

        [SerializeField] private string LastYearText = "last year";
        /// <summary> The text to use when the time difference is one year. </summary>
        public string lastYearText
        {
            get => LastYearText;
            set => LastYearText = value;
        }

        [SerializeField] private string NextYearText = "next year";
        /// <summary> The text to use when the time difference is one year. </summary>
        public string nextYearText
        {
            get => NextYearText;
            set => NextYearText = value;
        }

        /// <summary>
        /// Transforms a DateTime value before it is displayed in a UI component.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (!(source is DateTime dateTime)) return source;
            if (!enabled) return source;

            DateTime now = DateTime.UtcNow;
            TimeSpan timeDifference = dateTime - now;
            TimeSpan absoluteTimeDifference = timeDifference.Duration();

            string textFormat = timeDifference.TotalSeconds < 0 ? PastTextFormat : FutureTextFormat;
            string timeAgoText;

            if (absoluteTimeDifference <= TimeSpan.FromSeconds(1))
            {
                timeAgoText = JustNowText;
                return timeAgoText;
            }
            
            if (absoluteTimeDifference < TimeSpan.FromMinutes(1))
            {
                int seconds = absoluteTimeDifference.Seconds;
                timeAgoText = string.Format(textFormat, seconds == 1 ? $"1 {SecondTextSingular}" : $"{seconds} {SecondTextPlural}");
                return timeAgoText;
            }

            if (absoluteTimeDifference < TimeSpan.FromHours(1))
            {
                int minutes = absoluteTimeDifference.Minutes;
                timeAgoText = string.Format(textFormat, minutes == 1 ? $"1 {MinuteTextSingular}" : $"{minutes} {MinuteTextPlural}");
                return timeAgoText;
            }

            if (absoluteTimeDifference < TimeSpan.FromDays(1))
            {
                int hours = absoluteTimeDifference.Hours;
                timeAgoText = string.Format(textFormat, hours == 1 ? $"1 {HourTextSingular}" : $"{hours} {HourTextPlural}");
                return timeAgoText;
            }
            
            if (absoluteTimeDifference < TimeSpan.FromDays(7))
            {
                int days = absoluteTimeDifference.Days;
                if (timeDifference.TotalDays < 0)
                {
                    timeAgoText = days == 1 ? YesterdayText : string.Format(PastTextFormat, $"{days} {DayTextPlural}");
                    return timeAgoText;
                }
                
                timeAgoText = days == 1 ? TomorrowText : string.Format(FutureTextFormat, $"{days} {DayTextPlural}");
                return timeAgoText;
            }
            
            if (absoluteTimeDifference < TimeSpan.FromDays(30))
            {
                int weeks = (int)Math.Floor(absoluteTimeDifference.Days / 7.0);
                timeAgoText = timeDifference.TotalDays < 0 ? LastWeekText : NextWeekText;
                timeAgoText = string.Format(textFormat, weeks == 1 ? $"1 {WeekTextSingular}" : $"{weeks} {WeekTextPlural}", timeAgoText);
                return timeAgoText;
            }
            
            if (absoluteTimeDifference < TimeSpan.FromDays(365))
            {
                int months = (int)Math.Floor(absoluteTimeDifference.Days / 30.0);
                timeAgoText = timeDifference.TotalDays < 0 ? LastMonthText : NextMonthText;
                timeAgoText = string.Format(textFormat, months == 1 ? $"1 {MonthTextSingular}" : $"{months} {MonthTextPlural}", timeAgoText);
                return timeAgoText;
            }
            
            int years = (int)Math.Floor(absoluteTimeDifference.Days / 365.0);
            timeAgoText = timeDifference.TotalDays < 0 ? LastYearText : NextYearText;
            timeAgoText = string.Format(textFormat, years == 1 ? $"1 {YearTextSingular}" : $"{years} {YearTextPlural}", timeAgoText);
            return timeAgoText;
        }
    }
}
