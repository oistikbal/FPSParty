// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

namespace Doozy.Runtime.Bindy.Converters
{
    /// <summary>
    /// Converts a sprite to a texture2D.
    /// <example>
    /// <code>
    /// // Create a Sprite object
    /// Sprite sprite = Resources.Load&lt;Sprite&gt;("Sprites/my_sprite");
    ///
    /// // Convert the Sprite to a Texture2D
    /// IValueConverter converter = new SpriteToTexture2DConverter();
    /// Texture2D texture = (Texture2D)converter.Convert(sprite, typeof(Texture2D));
    /// </code>
    /// </example>
    /// </summary>
    public class SpriteToTexture2DConverter : IValueConverter
    {
        /// <summary>
        /// Flag that determines whether the converter should be registered to the converter registry refreshing the list of available converters.
        /// This is useful for special converters that are not registered to the converter registry by default.
        /// </summary>
        public bool registerToConverterRegistry => true;
        
        /// <summary>
        /// The source type to convert from.
        /// </summary>
        public Type sourceType => typeof(Sprite);

        /// <summary>
        /// The target type to convert to.
        /// </summary>
        public Type targetType => typeof(Texture2D);

        /// <summary>
        /// Determines whether the converter can convert between the specified source and target types.
        /// </summary>
        /// <param name="source"> The source type to convert from. </param>
        /// <param name="target"> The target type to convert to. </param>
        /// <returns> True if the conversion is supported, otherwise false. </returns>
        public bool CanConvert(Type source, Type target) =>
            source == typeof(Sprite) && target == typeof(Texture2D);

        /// <summary>
        /// Converts the specified value to the target type.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <param name="target"> The target type to convert to. </param>
        /// <returns> The converted value. </returns>
        public object Convert(object value, Type target)
        {
            if (value == null)
                return null;

            if (target != typeof(Texture2D))
                throw new ArgumentException($"Invalid target type: {target}. Expected: {typeof(Texture2D)}.");

            if (value is not Sprite sprite)
                throw new ArgumentException($"Invalid source type: {value.GetType()}. Expected: {typeof(Sprite)}.");

            var texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            texture.SetPixels(sprite.texture.GetPixels
                (
                    (int)sprite.textureRect.x, (int)sprite.textureRect.y,
                    (int)sprite.textureRect.width, (int)sprite.textureRect.height)
            );
            texture.Apply();
            return texture;

        }
    }
}
