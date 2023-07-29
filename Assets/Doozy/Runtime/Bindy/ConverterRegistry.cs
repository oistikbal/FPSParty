// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Runtime.Bindy.Converters;
using Doozy.Runtime.Common.Attributes;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable PartialTypeWithSinglePart

namespace Doozy.Runtime.Bindy
{
    /// <summary>
    /// A registry of value converters that can convert between two value types.
    /// </summary>
    public static partial class ConverterRegistry
    {
        private static readonly List<IValueConverter> Converters = new();
        private static readonly List<IValueConverterFactory> Factories = new();
        private static bool s_initialized;

        /// <summary>
        /// Adds a new value converter to the registry.
        /// </summary>
        /// <param name="converter"> The converter to add.</param>
        /// <exception cref="ArgumentNullException"> Thrown if the converter is null.</exception>
        public static void AddConverter(IValueConverter converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter), "Converter cannot be null.");

            if (ContainsConverter(converter.GetType()))
                return;

            Converters.Add(converter);
        }

        /// <summary>
        /// Checks if the registry contains a converter of the specified type
        /// </summary>
        /// <param name="converterType"> The converter type to check for </param>
        /// <returns> True if the registry contains a converter of the specified type, false otherwise.</returns>
        public static bool ContainsConverter(Type converterType) =>
            Converters.Exists(converter => converter.GetType() == converterType);

        /// <summary>
        /// Adds a new value converter factory to the registry.
        /// </summary>
        /// <param name="factory"> The factory to add.</param>
        /// <exception cref="ArgumentNullException"> Thrown if the factory is null.</exception>
        public static void AddFactory(IValueConverterFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory), "Factory cannot be null.");

            Factories.Add(factory);
        }

        /// <summary>
        /// Gets a value converter that can convert between the specified source and target types.
        /// If no suitable converter is found, null is returned.
        /// </summary>
        /// <param name="sourceType"> The source type to convert from.</param>
        /// <param name="targetType"> The target type to convert to.</param>
        /// <returns> A value converter, or null if no suitable converter is found.</returns>
        public static IValueConverter GetConverter(Type sourceType, Type targetType)
        {
            // search for a converter that can convert between the specified types
            for (int i = 0; i < Converters.Count; i++)
                if (Converters[i].CanConvert(sourceType, targetType))
                    return Converters[i];

            // a converter was not found, so search for a factory that can create a converter for the specified types
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (int i = 0; i < Factories.Count; i++)
                if (Factories[i].CanCreate(sourceType, targetType))
                    return Factories[i].Create(sourceType, targetType);
            
            return null;
        }

        /// <summary>
        /// Tries to get a value converter that can convert between the specified source and target types.
        /// If no suitable converter is found, false is returned.
        /// </summary>
        /// <param name="sourceType"> The source type to convert from.</param>
        /// <param name="targetType"> The target type to convert to.</param>
        /// <returns> A value converter, or null if no suitable converter is found.</returns>
        public static (bool converterFound, IValueConverter converter) TryGetConverter(Type sourceType, Type targetType)
        {
            // search for a converter that can convert between the specified types
            for (int i = 0; i < Converters.Count; i++)
                if (Converters[i].CanConvert(sourceType, targetType))
                    return (true, Converters[i]);

            // a converter was not found, so search for a factory that can create a converter for the specified types
            for (int i = 0; i < Factories.Count; i++)
                if (Factories[i].CanCreate(sourceType, targetType))
                    return (true, Factories[i].Create(sourceType, targetType));

            return (false, null);
        }



    }
}
