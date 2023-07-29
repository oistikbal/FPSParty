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
    [CustomEditor(typeof(PercentTransformer), true)]
    public class PercentTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;

        private SerializedProperty propertyFormatMode { get; set; }
        private SerializedProperty propertyPercentSymbol { get; set; }
        private SerializedProperty propertyAppendPercentSymbol { get; set; }
        private SerializedProperty propertyNegativeSymbol { get; set; }
        private SerializedProperty propertyAllowNegativePercentages { get; set; }
        private SerializedProperty propertyMinPercentValue { get; set; }
        private SerializedProperty propertyMaxPercentValue { get; set; }
        private SerializedProperty propertyUseMinMaxPercentValues { get; set; }
        private SerializedProperty propertyRoundToNearest { get; set; }
        private SerializedProperty propertyDecimalSeparator { get; set; }
        private SerializedProperty propertyDecimalPlaces { get; set; }
        private SerializedProperty propertyFormatString { get; set; }
        private SerializedProperty propertyUseFormatString { get; set; }

        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyFormatMode = serializedObject.FindProperty("FormatMode");
            propertyPercentSymbol = serializedObject.FindProperty("PercentSymbol");
            propertyAppendPercentSymbol = serializedObject.FindProperty("AppendPercentSymbol");
            propertyNegativeSymbol = serializedObject.FindProperty("NegativeSymbol");
            propertyAllowNegativePercentages = serializedObject.FindProperty("AllowNegativePercentages");
            propertyMinPercentValue = serializedObject.FindProperty("MinPercentValue");
            propertyMaxPercentValue = serializedObject.FindProperty("MaxPercentValue");
            propertyUseMinMaxPercentValues = serializedObject.FindProperty("UseMinMaxPercentValues");
            propertyRoundToNearest = serializedObject.FindProperty("RoundToNearest");
            propertyDecimalSeparator = serializedObject.FindProperty("DecimalSeparator");
            propertyDecimalPlaces = serializedObject.FindProperty("DecimalPlaces");
            propertyFormatString = serializedObject.FindProperty("FormatString");
            propertyUseFormatString = serializedObject.FindProperty("UseFormatString");
        }

        protected override void InitializeCustomInspector()
        {
            EnumField formatModeEnumField =
                DesignUtils.NewEnumField(propertyFormatMode)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The format mode for the percentage value.");

            FluidField formatModeFluidField =
                FluidField.Get()
                    .SetLabelText("Format Mode")
                    .AddFieldContent(formatModeEnumField);

            TextField percentSymbolTextField =
                DesignUtils.NewTextField(propertyPercentSymbol)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The symbol that will be appended to the formatted value.");

            percentSymbolTextField.SetEnabled(propertyAppendPercentSymbol.boolValue);

            FluidToggleCheckbox appendPercentSymbolToggle =
                FluidToggleCheckbox.Get()
                    .BindToProperty(propertyAppendPercentSymbol)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, the percent symbol will be appended to the formatted value.")
                    .SetOnValueChanged(evt => percentSymbolTextField.SetEnabled(evt.newValue));

            FluidField percentSymbolFluidField =
                FluidField.Get()
                    .SetLabelText("Percent Symbol")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .SetStyleAlignItems(Align.Center)
                            .AddChild(appendPercentSymbolToggle)
                            .AddSpaceBlock(2)
                            .AddChild(percentSymbolTextField)
                    );

            TextField negativeSymbolTextField =
                DesignUtils.NewTextField(propertyNegativeSymbol)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The symbol that will be appended to the formatted value if it is negative.");

            negativeSymbolTextField.SetEnabled(propertyAllowNegativePercentages.boolValue);

            FluidToggleCheckbox allowNegativePercentagesToggle =
                FluidToggleCheckbox.Get()
                    .BindToProperty(propertyAllowNegativePercentages)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, negative percentages will be allowed.")
                    .SetOnValueChanged(evt => negativeSymbolTextField.SetEnabled(evt.newValue));

            FluidField negativeSymbolFluidField =
                FluidField.Get()
                    .SetLabelText("Negative Symbol")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .SetStyleAlignItems(Align.Center)
                            .AddChild(allowNegativePercentagesToggle)
                            .AddSpaceBlock(2)
                            .AddChild(negativeSymbolTextField)
                    );

            FloatField minPercentValueFloatField =
                DesignUtils.NewFloatField(propertyMinPercentValue)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The minimum value for percentages. Values below this will be clamped.");

            minPercentValueFloatField.SetEnabled(propertyUseMinMaxPercentValues.boolValue);

            FloatField maxPercentValueFloatField =
                DesignUtils.NewFloatField(propertyMaxPercentValue)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The maximum value for percentages. Values above this will be clamped.");

            maxPercentValueFloatField.SetEnabled(propertyUseMinMaxPercentValues.boolValue);

            FluidToggleCheckbox useMinMaxPercentValuesToggle =
                FluidToggleCheckbox.Get()
                    .BindToProperty(propertyUseMinMaxPercentValues)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, the minimum and maximum values for percentages will be used.")
                    .SetOnValueChanged(evt =>
                    {
                        minPercentValueFloatField.SetEnabled(evt.newValue);
                        maxPercentValueFloatField.SetEnabled(evt.newValue);
                    });

            FluidField minMaxPercentValuesFluidField =
                FluidField.Get()
                    .SetLabelText("Min/Max Percent Values")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .SetStyleAlignItems(Align.Center)
                            .AddChild(useMinMaxPercentValuesToggle)
                            .AddSpaceBlock(2)
                            .AddChild(DesignUtils.NewFieldNameLabel("Min"))
                            .AddSpaceBlock()
                            .AddChild(minPercentValueFloatField)
                            .AddSpaceBlock(2)
                            .AddChild(DesignUtils.NewFieldNameLabel("Max"))
                            .AddSpaceBlock()
                            .AddChild(maxPercentValueFloatField)
                    );

            TextField decimalSeparatorTextField =
                DesignUtils.NewTextField(propertyDecimalSeparator)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The decimal separator symbol.");

            decimalSeparatorTextField.SetEnabled(propertyRoundToNearest.boolValue);

            IntegerField decimalPlacesIntegerField =
                DesignUtils.NewIntegerField(propertyDecimalPlaces)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The number of decimal places.");

            decimalPlacesIntegerField.SetEnabled(propertyRoundToNearest.boolValue);

            FluidToggleCheckbox roundToNearestToggle =
                FluidToggleCheckbox.Get()
                    .BindToProperty(propertyRoundToNearest)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE,  values will be rounded to the nearest decimal place.")
                    .SetOnValueChanged(evt =>
                    {
                        decimalSeparatorTextField.SetEnabled(evt.newValue);
                        decimalPlacesIntegerField.SetEnabled(evt.newValue);
                    });

            FluidField roundToNearestFluidField =
                FluidField.Get()
                    .SetLabelText("Round To Nearest")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .SetStyleAlignItems(Align.Center)
                            .AddChild(roundToNearestToggle)
                            .AddSpaceBlock(2)
                            .AddChild(DesignUtils.NewFieldNameLabel("Decimal Separator"))
                            .AddSpaceBlock()
                            .AddChild(decimalSeparatorTextField)
                            .AddSpaceBlock(2)
                            .AddChild(DesignUtils.NewFieldNameLabel("Decimal Places"))
                            .AddSpaceBlock()
                            .AddChild(decimalPlacesIntegerField)
                    );

            TextField formatStringTextField =
                DesignUtils.NewTextField(propertyFormatString)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The format string to use for formatting the value.");

            formatStringTextField.SetEnabled(propertyUseFormatString.boolValue);

            FluidToggleCheckbox useFormatStringToggle =
                FluidToggleCheckbox.Get()
                    .BindToProperty(propertyUseFormatString)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, the format string will be used for formatting the value.")
                    .SetOnValueChanged(evt => formatStringTextField.SetEnabled(evt.newValue));

            FluidField formatStringFluidField =
                FluidField.Get()
                    .SetLabelText("Format String")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .SetStyleAlignItems(Align.Center)
                            .AddChild(useFormatStringToggle)
                            .AddSpaceBlock(2)
                            .AddChild(formatStringTextField)
                    );

            contentContainer
                .AddChild(formatModeFluidField)
                .AddSpaceBlock()
                .AddChild(percentSymbolFluidField)
                .AddSpaceBlock()
                .AddChild(negativeSymbolFluidField)
                .AddSpaceBlock()
                .AddChild(minMaxPercentValuesFluidField)
                .AddSpaceBlock()
                .AddChild(roundToNearestFluidField)
                .AddSpaceBlock()
                .AddChild(formatStringFluidField);

        }
    }
}
