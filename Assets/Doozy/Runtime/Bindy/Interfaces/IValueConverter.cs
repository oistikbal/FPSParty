// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

namespace Doozy.Runtime.Bindy
{
    /// <summary>
    /// Defines an interface for value converters that can convert between two value types.
    /// </summary>
    public interface IValueConverter
    {
        /// <summary>
        /// Flag that determines whether the converter should be registered to the converter registry refreshing the list of available converters.
        /// This is useful for special converters that are not registered to the converter registry by default.
        /// </summary>
        bool registerToConverterRegistry { get; }
        
        /// <summary>
        /// Gets the source type of the conversion.
        /// </summary>
        Type sourceType { get; }

        /// <summary>
        /// Gets the target type of the conversion.
        /// </summary>
        Type targetType { get; }

        /// <summary>
        /// Determines whether the converter can convert between the specified source and target types.
        /// </summary>
        /// <param name="source">The source type to convert from.</param>
        /// <param name="target">The target type to convert to.</param>
        /// <returns>True if the conversion is supported, otherwise false.</returns>
        bool CanConvert(Type source, Type target);

        /// <summary>
        /// Converts the specified value to the target type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="target">The target type to convert to.</param>
        /// <returns>The converted value.</returns>
        object Convert(object value, Type target);
    }
}