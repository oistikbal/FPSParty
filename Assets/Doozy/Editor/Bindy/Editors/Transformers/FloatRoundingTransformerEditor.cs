// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Bindy.Transformers;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;


namespace Doozy.Editor.Bindy.Editors.Transformers
{
    [CustomEditor(typeof(FloatRoundingTransformer), true)]
    public class FloatRoundingTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;
        
        private SerializedProperty propertyDecimalPlaces { get; set; }
        
        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyDecimalPlaces = serializedObject.FindProperty("DecimalPlaces");
        }
        
        protected override void InitializeCustomInspector()
        {
            UnityEngine.UIElements.IntegerField decimalPlacesField =
                DesignUtils.NewIntegerField(propertyDecimalPlaces)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The number of decimal places to round the float value to");
            
            FluidField decimalPlacesFluidField =
                FluidField.Get()
                    .SetLabelText("Decimal Places")
                    .AddFieldContent(decimalPlacesField);
            
            contentContainer
                .AddChild(decimalPlacesFluidField);
        }
    }
}
