// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
// ReSharper disable InconsistentNaming

namespace Doozy.Runtime.Bindy.Converters
{
    /// <summary>
    /// Converts an enum value to a string value.
    /// </summary>
    /// <example>
    /// <code>
    /// MyEnum myEnumValue = MyEnum.SomeValue;
    /// var converter = new EnumToStringConverter&lt;MyEnum&gt;();
    /// string stringValue = (string)converter.Convert(myEnumValue, typeof(string));
    /// </code>
    /// </example>
    public class EnumToStringConverter : IValueConverter
    {
        /// <summary>
        /// Flag that determines whether the converter should be registered to the converter registry refreshing the list of available converters.
        /// This is useful for special converters that are not registered to the converter registry by default.
        /// </summary>
        public bool registerToConverterRegistry => true;
        
        /// <summary>
        /// The source type to convert from.
        /// </summary>
        public Type sourceType => typeof(Enum);

        /// <summary>
        /// The target type to convert to.
        /// </summary>
        public Type targetType => typeof(string);

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
            return source.IsEnum && target == targetType;
        }

        /// <summary>
        /// Converts the specified value to the target type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="target">The target type to convert to.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentException">Thrown if the value is not of the expected source type.</exception>
        public object Convert(object value, Type target)
        {
            if (value == null)
                return null;

            if (target != targetType)
                throw new ArgumentException($"Invalid target type: {target}. Expected: {targetType}.");

            if (value is Enum enumValue)
                return enumValue.ToString();

            throw new ArgumentException($"Invalid source type: {value.GetType()}. Expected: {sourceType}.");
        }
    }

}
