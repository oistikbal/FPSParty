// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

namespace Doozy.Runtime.Bindy.Converters
{
    /// <summary>
    /// Converts a string value to an integer value.
    /// </summary>
    public class StringToIntConverter : IValueConverter
    {
        /// <summary>
        /// Flag that determines whether the converter should be registered to the converter registry refreshing the list of available converters.
        /// This is useful for special converters that are not registered to the converter registry by default.
        /// </summary>
        public bool registerToConverterRegistry => true;
        
        /// <summary>
        /// The source type to convert from.
        /// </summary>
        public Type sourceType => typeof(string);

        /// <summary>
        /// The target type to convert to.
        /// </summary>
        public Type targetType => typeof(int);

        /// <summary>
        /// Determines whether the converter can convert between the specified source and target types.
        /// </summary>
        /// <param name="source">The source type to convert from.</param>
        /// <param name="target">The target type to convert to.</param>
        /// <returns>True if the conversion is supported, otherwise false.</returns>
        public bool CanConvert(Type source, Type target) =>
            source == typeof(string) && target == typeof(int);

        /// <summary>
        /// Converts the specified value to the target type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="target">The target type to convert to.</param>
        /// <returns>The converted value.</returns>
        public object Convert(object value, Type target)
        {
            if (value == null)
                return 0;

            if (target != typeof(int))
                throw new ArgumentException($"Invalid target type: {target}. Expected: {typeof(int)}.");

            if (value is not string strValue)
                throw new ArgumentException($"Invalid source type: {value.GetType()}. Expected: {typeof(string)}.");
            
            if (int.TryParse(strValue, out int intValue))
                return intValue;
                
            throw new ArgumentException($"Invalid source value: {strValue}. Expected: {typeof(int)}.");
        }
    }
}