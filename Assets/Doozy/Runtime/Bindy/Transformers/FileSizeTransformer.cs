// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Bindy.Transformers
{
    /// <summary>
    /// Transforms a file size value (in bytes) as a human-readable string, with options for different units (e.g. KB, MB, GB).
    /// <para/> The transformer supports the following options:
    /// <para/> UnitSize (int, default: 1024): The base unit size (e.g. 1 KB = 1024 bytes).
    /// <para/> UnitSuffixes (string[], default: ["B", "KB", "MB", "GB", "TB"]): The available units and their suffixes.
    /// </summary>
    [CreateAssetMenu(fileName = "File Size", menuName = "Doozy/Bindy/Transformer/File Size", order = -950)]
    public class FileSizeTransformer : ValueTransformer
    {
        public override string description =>
            "Transforms a file size value (in bytes) as a human-readable string, with options for different units (e.g. KB, MB, GB).\n\n" +
            "The transformer supports the following options:\n" +
            "UnitSize (int, default: 1024): The base unit size (e.g. 1 KB = 1024 bytes).\n" +
            "UnitSuffixes (string[], default: [\"B\", \"KB\", \"MB\", \"GB\", \"TB\"]): The available units and their suffixes.";
        
        protected override Type[] fromTypes => new[] { typeof(long) };
        protected override Type[] toTypes => new[] { typeof(string) };

        /// <summary>
        /// The size of the unit (in bytes) that will be used to calculate the file size.
        /// The base unit size (e.g. 1 KB = 1024 bytes).
        /// </summary>
        [SerializeField] private int UnitSize = 1024;
        public int unitSize
        {
            get => UnitSize;
            set => UnitSize = value;
        }

        /// <summary>
        /// The available units and their suffixes.
        /// </summary>
        [SerializeField] private string[] UnitSuffixes = { "B", "KB", "MB", "GB", "TB" };
        public string[] unitSuffixes
        {
            get => UnitSuffixes;
            set => UnitSuffixes = value;
        }

        /// <summary>
        /// Transforms a file size value as a human-readable string with the appropriate unit suffix.
        /// </summary>
        /// <param name="source"> Source value </param>
        /// <param name="target"> Target value </param>
        /// <returns> Transformed value </returns>
        public override object Transform(object source, object target)
        {
            if (source == null) return null;
            if (!enabled) return source;

            if (source is not long fileSize)
                return source;

            // Calculate the file size in the appropriate unit
            double size = fileSize;
            int unitIndex = 0;
            while (size >= unitSize && unitIndex < unitSuffixes.Length - 1)
            {
                size /= unitSize;
                unitIndex++;
            }

            // Format the size as a string with the appropriate suffix
            string formattedSize = $"{size:0.#} {unitSuffixes[unitIndex]}";

            return formattedSize;
        }
    }
}
