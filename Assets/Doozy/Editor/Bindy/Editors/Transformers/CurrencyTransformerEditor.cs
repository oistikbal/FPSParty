// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Bindy.Transformers;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Doozy.Editor.Bindy.Editors.Transformers
{
    [CustomEditor(typeof(CurrencyTransformer), true)]
    public class CurrencyTransformerEditor: ValueTransformerEditor
    {
        protected override bool customEditor => true;
        
        private SerializedProperty propertySymbolPosition { get; set; }
        private SerializedProperty propertyCurrencySymbol { get; set; }
        private SerializedProperty propertyGroupSeparator { get; set; }
        private SerializedProperty propertyDecimalSeparator { get; set; }
        private SerializedProperty propertyDecimalDigits { get; set; }
        
        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertySymbolPosition = serializedObject.FindProperty("SymbolPosition");
            propertyCurrencySymbol = serializedObject.FindProperty("CurrencySymbol");
            propertyGroupSeparator = serializedObject.FindProperty("GroupSeparator");
            propertyDecimalSeparator = serializedObject.FindProperty("DecimalSeparator");
            propertyDecimalDigits = serializedObject.FindProperty("DecimalDigits");
        }
        
        protected override void InitializeCustomInspector()
        {
            EnumField symbolPositionEnumField =
                DesignUtils.NewEnumField(propertySymbolPosition)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The position of the currency symbol");
            
            FluidField symbolPositionFluidField =
                FluidField.Get()
                    .SetLabelText("Symbol Position")
                    .AddFieldContent(symbolPositionEnumField);

                TextField currencySymbolTextField =
                DesignUtils.NewTextField(propertyCurrencySymbol)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The currency symbol to use");
            
            FluidField currencySymbolFluidField =
                FluidField.Get()
                    .SetLabelText("Currency Symbol")
                    .AddFieldContent(currencySymbolTextField);
            
            TextField groupSeparatorTextField =
                DesignUtils.NewTextField(propertyGroupSeparator)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The group separator to use");
            
            FluidField groupSeparatorFluidField =
                FluidField.Get()
                    .SetLabelText("Group Separator")
                    .AddFieldContent(groupSeparatorTextField);
            
            TextField decimalSeparatorTextField =
                DesignUtils.NewTextField(propertyDecimalSeparator)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The decimal separator to use");
            
            FluidField decimalSeparatorFluidField =
                FluidField.Get()
                    .SetLabelText("Decimal Separator")
                    .AddFieldContent(decimalSeparatorTextField);
            
            IntegerField decimalDigitsIntegerField =
                DesignUtils.NewIntegerField(propertyDecimalDigits)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The number of decimal digits to use");
            
            FluidField decimalDigitsFluidField =
                FluidField.Get()
                    .SetLabelText("Decimal Digits")
                    .AddFieldContent(decimalDigitsIntegerField);
            
            contentContainer
                .AddChild(symbolPositionFluidField)
                .AddSpaceBlock()
                .AddChild(currencySymbolFluidField)
                .AddSpaceBlock()
                .AddChild(groupSeparatorFluidField)
                .AddSpaceBlock()
                .AddChild(decimalSeparatorFluidField)
                .AddSpaceBlock()
                .AddChild(decimalDigitsFluidField);
        }
    }
}
