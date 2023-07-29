// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Bindy.Transformers;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Doozy.Editor.Bindy.Editors.Transformers
{
    [CustomEditor(typeof(Vector3Transformer), true)]
    public class Vector3TransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;

        private SerializedProperty propertyDecimalPlaces { get; set; }
        private SerializedProperty propertyRoundX { get; set; }
        private SerializedProperty propertyRoundY { get; set; }
        private SerializedProperty propertyRoundZ { get; set; }
        private SerializedProperty propertyDisplayFormat { get; set; }

        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyDecimalPlaces = serializedObject.FindProperty("DecimalPlaces");
            propertyRoundX = serializedObject.FindProperty("RoundX");
            propertyRoundY = serializedObject.FindProperty("RoundY");
            propertyRoundZ = serializedObject.FindProperty("RoundZ");
            propertyDisplayFormat = serializedObject.FindProperty("DisplayFormat");
        }

        protected override void InitializeCustomInspector()
        {
            IntegerField decimalPlacesField =
                DesignUtils.NewIntegerField(propertyDecimalPlaces)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The number of decimal places to round to.");

            FluidField decimalPlacesFluidField =
                FluidField.Get()
                    .SetLabelText("Decimal Places")
                    .AddFieldContent(decimalPlacesField);

            FluidToggleCheckbox roundXToggle =
                FluidToggleCheckbox.Get()
                    .BindToProperty(propertyRoundX)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetLabelText("X")
                    .SetTooltip("Round the x component of the Vector3 value to the specified number of decimal places");

            FluidToggleCheckbox roundYToggle =
                FluidToggleCheckbox.Get()
                    .BindToProperty(propertyRoundY)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetLabelText("Y")
                    .SetTooltip("Round the y component of the Vector3 value to the specified number of decimal places");

            FluidToggleCheckbox roundZToggle =
                FluidToggleCheckbox.Get()
                    .BindToProperty(propertyRoundZ)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetLabelText("Z")
                    .SetTooltip("Round the z component of the Vector3 value to the specified number of decimal places");

            FluidField roundField =
                FluidField.Get()
                    .SetLabelText("Round")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .SetStyleAlignItems(Align.Center)
                            .AddChild(roundXToggle)
                            .AddSpaceBlock()
                            .AddChild(roundYToggle)
                            .AddSpaceBlock()
                            .AddChild(roundZToggle)
                    );

            TextField displayFormatField =
                DesignUtils.NewTextField(propertyDisplayFormat)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The display format of the Vector3 value. Use the following placeholders: {0} = x, {1} = y, {2} = z");

            FluidField displayFormatFluidField =
                FluidField.Get()
                    .SetLabelText("Display Format")
                    .AddFieldContent(displayFormatField);

            contentContainer
                .AddChild(decimalPlacesFluidField)
                .AddSpaceBlock()
                .AddChild(roundField)
                .AddSpaceBlock()
                .AddChild(displayFormatFluidField);
        }
    }
}
