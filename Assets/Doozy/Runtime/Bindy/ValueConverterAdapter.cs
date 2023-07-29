// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
// ReSharper disable UnusedType.Global

namespace Doozy.Runtime.Bindy
{
    /// <summary>
    /// Adapts an existing <see cref="IValueConverter"/> implementation to the <see cref="IValueConverterFactory"/> interface.
    /// </summary>
    public class ValueConverterAdapter : IValueConverterFactory
    {
        private readonly IValueConverter m_Converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConverterAdapter"/> class with the specified converter to adapt.
        /// </summary>
        /// <param name="converter">The converter to adapt.</param>
        public ValueConverterAdapter(IValueConverter converter) =>
            m_Converter = converter;

        /// <summary>
        /// Determines whether the factory can create a converter for the specified source and target type by checking whether the source and target types match the converter's <see cref="IValueConverter.sourceType"/> and <see cref="IValueConverter.targetType"/>.
        /// </summary>
        /// <param name="sourceType">The source type to convert from.</param>
        /// <param name="targetType">The target type to convert to.</param>
        /// <returns>True if the factory can create a converter for the specified types; otherwise, false.</returns>
        public bool CanCreate(Type sourceType, Type targetType) =>
            m_Converter.sourceType == sourceType && m_Converter.targetType == targetType;

        /// <summary>
        /// Creates a new instance of the <see cref="IValueConverter"/> implementation adapted by this factory if the source and target types match the converter's <see cref="IValueConverter.sourceType"/> and <see cref="IValueConverter.targetType"/>; otherwise, returns null.
        /// </summary>
        /// <param name="sourceType">The source type to convert from.</param>
        /// <param name="targetType">The target type to convert to.</param>
        /// <returns>A new instance of the <see cref="IValueConverter"/> implementation adapted by this factory if the source and target types match the converter's <see cref="IValueConverter.sourceType"/> and <see cref="IValueConverter.targetType"/>; otherwise, null.</returns>
        public IValueConverter Create(Type sourceType, Type targetType) =>
            CanCreate(sourceType, targetType) ? m_Converter : null;
    }
}
