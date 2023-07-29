// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

namespace Doozy.Runtime.Bindy.Converters
{
    /// <summary>
    /// Converts a string to a Vector3 value.
    /// This implementation converts the string to a Vector3 value using the Vector3.Parse method.
    /// <para/> Note that it throws an ArgumentException if the source or target types are not of the expected type,
    /// or if the string is not a valid representation of a Vector3.
    /// </summary>
    public class StringToVector3Converter : IValueConverter
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
        public Type targetType => typeof(Vector3);

        /// <summary>
        /// Determines whether the converter can convert between the specified source and target types.
        /// </summary>
        /// <param name="source">The source type to convert from.</param>
        /// <param name="target">The target type to convert to.</param>
        /// <returns>True if the conversion is supported, otherwise false.</returns>
        public bool CanConvert(Type source, Type target) =>
            source == typeof(string) && target == typeof(Vector3);

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

            if (target != typeof(Vector3))
                throw new ArgumentException($"Invalid target type: {target}. Expected: {typeof(Vector3)}.");

            if (value is not string stringValue)
                throw new ArgumentException($"Cannot convert value '{value}' to type '{target}'.");
            
            string[] components = stringValue.Split(',');
            if (components.Length == 3 && 
                float.TryParse(components[0], out float x) &&
                float.TryParse(components[1], out float y) &&
                float.TryParse(components[2], out float z))
            {
                return new Vector3(x, y, z);
            }

            throw new ArgumentException($"Cannot convert value '{value}' to type '{target}'.");
        }
    }
}
