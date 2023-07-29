// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

namespace Doozy.Runtime.Bindy
{
    /// <summary>
    /// Defines an interface for factories that can create value converters for a given source and target type.
    /// </summary>
    public interface IValueConverterFactory
    {
        /// <summary>
        /// Determines whether the factory can create a converter for the specified source and target type.
        /// </summary>
        /// <param name="sourceType">The source type to convert from.</param>
        /// <param name="targetType">The target type to convert to.</param>
        /// <returns>True if the factory can create a converter, otherwise false.</returns>
        bool CanCreate(Type sourceType, Type targetType);

        /// <summary>
        /// Creates a new value converter for the specified source and target type.
        /// </summary>
        /// <param name="sourceType">The source type to convert from.</param>
        /// <param name="targetType">The target type to convert to.</param>
        /// <returns>A new value converter.</returns>
        IValueConverter Create(Type sourceType, Type targetType);
    }
}
