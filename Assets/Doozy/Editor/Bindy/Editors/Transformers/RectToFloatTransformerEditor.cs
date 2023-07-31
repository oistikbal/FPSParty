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
    [CustomEditor(typeof(RectToFloatTransformer), true)]
    public class RectToFloatTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;
        
        private SerializedProperty propertyComponent { get; set; }
        private SerializedProperty propertyDecimalPlaces { get; set; }
        
        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyComponent = serializedObject.FindProperty("Component");
            propertyDecimalPlaces = serializedObject.FindProperty("DecimalPlaces");
        }
        
        protected override void InitializeCustomInspector()
        {
            UnityEngine.UIElements.EnumField componentEnumField =
                DesignUtils.NewEnumField(propertyComponent)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The component of the Rect value to return as a float");
            
            FluidField componentFluidField =
                FluidField.Get()
                    .SetLabelText("Component")
                    .AddFieldContent(componentEnumField);

            UnityEngine.UIElements.IntegerField decimalPlacesIntegerField =
               DesignUtils.NewIntegerField(propertyDecimalPlaces)
                        .SetStyleFlexGrow(1)
                        .SetTooltip("The number of decimal places to round the float value to");
            
            FluidField decimalPlacesFluidField =
                FluidField.Get()
                    .SetLabelText("Decimal Places")
                    .AddFieldContent(decimalPlacesIntegerField);
            
            contentContainer
                .AddChild(componentFluidField)
                .AddSpaceBlock()
                .AddChild(decimalPlacesFluidField);
        }
    }
}
