// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.Bindy.Converters
{
    /// <summary>
    /// Converts a DateTime value to a string value using the specified format and vice-versa.
    /// </summary>
    /// <example>
    /// <code>
    /// DateTime dateTime = DateTime.Now;
    /// IValueConverter dateTimeConverter = new DateTimeToStringConverter("yyyy-MM-dd HH:mm:ss");
    ///
    /// //Convert DateTime to string
    /// string dateTimeString = (string)dateTimeConverter.Convert(dateTime, typeof(string));
    /// Debug.Log(dateTimeString);
    ///
    /// //Convert string back to DateTime
    /// DateTime parsedDateTime = (DateTime)dateTimeConverter.ConvertBack(dateTimeString, typeof(DateTime));
    /// Debug.Log(parsedDateTime);
    /// </code>
    /// </example>
    public class DateTimeToStringConverter : IValueConverter
    {
        /// <summary>
        /// Flag that determines whether the converter should be registered to the converter registry refreshing the list of available converters.
        /// This is useful for special converters that are not registered to the converter registry by default.
        /// </summary>
        public bool registerToConverterRegistry => true;
        
        /// <summary>
        /// The source type to convert from.
        /// </summary>
        public Type sourceType => typeof(DateTime);

        /// <summary>
        /// The target type to convert to.
        /// </summary>
        public Type targetType => typeof(string);

        /// <summary>
        /// The format to use when converting a DateTime to a string.
        /// </summary>
        private readonly string m_Format;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeToStringConverter"/> class with the default format.
        /// </summary>
        public DateTimeToStringConverter() : this("yyyy-MM-dd HH:mm:ss")
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeToStringConverter"/> class.
        /// </summary>
        /// <param name="format">The format to use when converting a DateTime to a string.</param>
        public DateTimeToStringConverter(string format)
        {
            m_Format = format;
        }

        /// <summary>
        /// Determines whether the converter can convert between the specified source and target types.
        /// </summary>
        /// <param name="source">The source type to convert from.</param>
        /// <param name="target">The target type to convert to.</param>
        /// <returns>True if the conversion is supported, otherwise false.</returns>
        public bool CanConvert(Type source, Type target) =>
            source == typeof(DateTime) && target == typeof(string);

        /// <summary>
        /// Converts the specified value to the target type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="target">The target type to convert to.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentException"></exception>
        public object Convert(object value, Type target)
        {
            if (value == null)
                return null;

            if (target != typeof(string))
                throw new ArgumentException($"Invalid target type: {target}. Expected: {typeof(string)}.");

            if (value is DateTime dateTimeValue)
                return dateTimeValue.ToString(m_Format);

            throw new ArgumentException($"Invalid source type: {value.GetType()}. Expected: {typeof(DateTime)}.");
        }
    }
}
