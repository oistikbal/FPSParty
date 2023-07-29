// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Bindy.Transformers;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine.UIElements;

namespace Doozy.Editor.Bindy.Editors.Transformers
{
    [CustomEditor(typeof(FloatToVector2Transformer), true)]
    public class FloatToVector2TransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;

        private SerializedProperty propertySetX { get; set; }
        private SerializedProperty propertySetY { get; set; }

        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertySetX = serializedObject.FindProperty("SetX");
            propertySetY = serializedObject.FindProperty("SetY");
        }

        protected override void InitializeCustomInspector()
        {
            FluidToggleCheckbox setXToggle =
                FluidToggleCheckbox.Get()
                    .BindToProperty(propertySetX)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetLabelText("X")
                    .SetTooltip("Set the x component of the Vector2 value to the float value");

            FluidToggleCheckbox setYToggle =
                FluidToggleCheckbox.Get()
                    .BindToProperty(propertySetY)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetLabelText("Y")
                    .SetTooltip("Set the y component of the Vector2 value to the float value");

            FluidField optionsField =
                FluidField.Get()
                    .SetLabelText("Set")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .SetStyleAlignItems(Align.Center)
                            .AddChild(setXToggle)
                            .AddSpaceBlock()
                            .AddChild(setYToggle)
                    );
            
            contentContainer
                .AddChild(optionsField);
        }
    }
}
