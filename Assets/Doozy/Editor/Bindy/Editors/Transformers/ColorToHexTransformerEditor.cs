// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Bindy.Transformers;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;

namespace Doozy.Editor.Bindy.Editors.Transformers
{
    [CustomEditor(typeof(ColorToHexTransformer), true)]
    public class ColorToHexTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;
        
        private SerializedProperty propertyIncludeHashSymbol { get; set; }
        private SerializedProperty propertyExcludeAlphaValue { get; set; }
        
        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyIncludeHashSymbol = serializedObject.FindProperty("IncludeHashSymbol");
            propertyExcludeAlphaValue = serializedObject.FindProperty("ExcludeAlphaValue");
        }

        protected override void InitializeCustomInspector()
        {
            FluidToggleCheckbox includeHashSymbolToggle =
                FluidToggleCheckbox.Get()
                    .BindToProperty(propertyIncludeHashSymbol)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetLabelText("Include Hash Symbol")
                    .SetTooltip("Include the hash symbol (#) in the returned hex string");
            
            FluidToggleCheckbox excludeAlphaValueToggle =
                FluidToggleCheckbox.Get()
                    .BindToProperty(propertyExcludeAlphaValue)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetLabelText("Exclude Alpha Value")
                    .SetTooltip("Exclude the alpha value from the returned hex string");
            
            contentContainer
                .AddChild(includeHashSymbolToggle)
                .AddSpaceBlock()
                .AddChild(excludeAlphaValueToggle);
        }
    }
}
