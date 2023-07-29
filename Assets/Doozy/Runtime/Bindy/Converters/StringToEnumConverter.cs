// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
// ReSharper disable InconsistentNaming

namespace Doozy.Runtime.Bindy.Converters
{
    /// <summary>
    /// Converts a string value to an enum value.
    /// </summary>
    /// <example>
    /// <code>
    /// // Create a new converter instance for the MyEnum type
    /// var converter = new StringToEnumConverter&lt;MyEnum&gt;();
    ///
    /// // Convert a string value to a MyEnum value
    /// string stringValue = "Value1";
    /// MyEnum enumValue = (MyEnum)converter.Convert(stringValue, typeof(MyEnum));
    /// </code>
    /// </example>
    public class StringToEnumConverter : IValueConverter
    {
        /// <summary>
        /// Flag that determines whether the converter should be registered to the converter registry refreshing the list of available converters.
        /// This is useful for special converters that are not registered to the converter registry by default.
        /// </summary>
        public bool registerToConverterRegistry => false;

        /// <summary>
        /// Gets the source type of the conversion.
        /// </summary>
        public Type sourceType => typeof(string);

        /// <summary>
        /// Gets the target type of the conversion.
        /// </summary>
        public Type targetType => typeof(Enum);

        /// <summary>
        /// Determines whether the converter can convert between the specified source and target types.
        /// </summary>
        /// <param name="source">The source type to convert from.</param>
        /// <param name="target">The target type to convert to.</param>
        /// <returns>True if the conversion is supported, otherwise false.</returns>
        public bool CanConvert(Type source, Type target)
        {
            if (source == null) return false;
            if (target == null) return false;
            return source == sourceType && target.IsEnum;
        }

        /// <summary>
        /// Converts the specified value to the target type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="target">The target type to convert to.</param>
        /// <returns>The converted value.</returns>
        public object Convert(object value, Type target)
        {
            if (value == null)
                return null;

            if (target != targetType)
                throw new ArgumentException($"Invalid target type: {target}. Expected: {targetType}.");

            object result = ParseEnum(target, value);

            if (result == null)
                throw new ArgumentException($"Cannot convert value '{value}' to type '{targetType}'.");

            return result;
        }

        private static object ParseEnum(Type enumType, object value)
        {
            if (enumType.IsEnum && value is string stringValue && Enum.TryParse(enumType, stringValue, out object result))
                return result;
            return null;
        }
    }
}
