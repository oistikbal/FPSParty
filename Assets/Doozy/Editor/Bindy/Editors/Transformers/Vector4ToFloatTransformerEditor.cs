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
    [CustomEditor(typeof(Vector4ToFloatTransformer), true)]
    public class Vector4ToFloatTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;
        
        private SerializedProperty propertyAxis { get; set; }
        private SerializedProperty propertyDecimalPlaces { get; set; }
        
        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyAxis = serializedObject.FindProperty("Axis");
            propertyDecimalPlaces = serializedObject.FindProperty("DecimalPlaces");
        }
        
        protected override void InitializeCustomInspector()
        {
            UnityEngine.UIElements.EnumField axisEnumField =
                DesignUtils.NewEnumField(propertyAxis)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The axis to use when converting the Vector4 value to a float value");
            
            FluidField axisFluidField =
                FluidField.Get()
                    .SetLabelText("Axis")
                    .AddFieldContent(axisEnumField);
            
            UnityEngine.UIElements.IntegerField decimalPlacesIntegerField =
                DesignUtils.NewIntegerField(propertyDecimalPlaces)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The number of decimal places to use when converting the Vector4 value to a float value");
            
            FluidField decimalPlacesFluidField =
                FluidField.Get()
                    .SetLabelText("Decimal Places")
                    .AddFieldContent(decimalPlacesIntegerField);
            
            contentContainer
                .AddChild(axisFluidField)
                .AddSpaceBlock()
                .AddChild(decimalPlacesFluidField);
        }
    }
}
