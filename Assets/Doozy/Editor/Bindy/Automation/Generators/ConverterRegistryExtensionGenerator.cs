// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Doozy.Editor.Common.Utils;
using Doozy.Runtime;
using Doozy.Runtime.Bindy;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Bindy.Automation.Generators
{
    internal static class ConverterRegistryExtensionGenerator
    {
        [RefreshData("Bindy Converters")]
        public static void RefreshData() =>
            Run();
        
        [MenuItem("Tools/Doozy/Refresh/Bindy Converters", priority = -450)]
        private static void Tester() => Run();

        private static string templateName => nameof(ConverterRegistryExtensionGenerator).Replace("Generator", "");
        private static string templateNameWithExtension => $"{templateName}.cst";
        private static string templateFilePath => $"{EditorPath.path}/Bindy/Automation/Templates/{templateNameWithExtension}";

        private static string targetFileNameWithExtension => $"{templateName}.cs";
        private static string targetFilePath => $"{RuntimePath.path}/Bindy/{targetFileNameWithExtension}";

        private static StringBuilder sb { get; set; }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private static bool Run(bool saveAssets = true, bool refreshAssetDatabase = false)
        {
            string data = FileGenerator.GetFile(templateFilePath);
            if (data.IsNullOrEmpty()) return false;
            sb = new StringBuilder(data);
            sb.Clear();
            var typesThatImplementInterface = GetTypesThatImplementIValueConverter().ToList();
            if (typesThatImplementInterface.Count == 0)
            {
                Debug.Log("[Bindy] - Converter Registry could not be refreshed. No converters found");
                return false;
            }
            foreach (Type type in typesThatImplementInterface.OrderBy(t => t.Name))
                sb.AppendLine($"            AddConverter(new {type.FullName}());");

            data = data.Replace("//Converters//", sb.ToString().RemoveLast(Environment.NewLine.Length));
            data += Environment.NewLine;
            
            bool result = FileGenerator.WriteFile(targetFilePath, data);
            if (!result)
            {
                Debug.Log
                (
                    "[Bindy] - Converter Registry could not be refreshed. " +
                    $"Something went wrong while writing the file at path: {targetFilePath}" +
                    $"Check the '{nameof(ConverterRegistryExtensionGenerator)}' class for more details."
                );
                return false;
            }
            if (saveAssets) AssetDatabase.SaveAssets();
            if (refreshAssetDatabase) AssetDatabase.Refresh();
            Debug.Log($"[Bindy] - Converter Registry has been refreshed. Found {typesThatImplementInterface.Count} converters.");
            return true;
        }

        /// <summary>
        /// Get all types that implement the IValueConverter interface and have the registerToConverterRegistry flag set to true 
        /// </summary>
        private static IEnumerable<Type> GetTypesThatImplementIValueConverter() =>
            ReflectionUtils.domainAssemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where
                (
                    type =>
                        typeof(IValueConverter).IsAssignableFrom(type) &&                             // Check if the type implements the IValueConverter interface
                        !type.IsInterface &&                                                          // Exclude interfaces as we are looking for concrete types
                        !type.ContainsGenericParameters &&                                            // Exclude types with generic parameters
                        ((IValueConverter)Activator.CreateInstance(type)).registerToConverterRegistry // Check if the registerToConverterRegistry flag is set to true for the type
                );
    }
}
