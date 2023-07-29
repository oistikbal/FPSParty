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
    [CustomEditor(typeof(RangeMappingTransformer), true)]
    public class RangeMappingTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;

        private SerializedProperty propertyInputMin { get; set; }
        private SerializedProperty propertyInputMax { get; set; }
        private SerializedProperty propertyOutputMin { get; set; }
        private SerializedProperty propertyOutputMax { get; set; }

        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyInputMin = serializedObject.FindProperty("InputMin");
            propertyInputMax = serializedObject.FindProperty("InputMax");
            propertyOutputMin = serializedObject.FindProperty("OutputMin");
            propertyOutputMax = serializedObject.FindProperty("OutputMax");
        }

        protected override void InitializeCustomInspector()
        {
            FloatField inputMinFloatField =
                DesignUtils.NewFloatField(propertyInputMin)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The minimum value of the input range");

            FloatField inputMaxFloatField =
                DesignUtils.NewFloatField(propertyInputMax)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The maximum value of the input range");

            FluidField inputMinMaxFluidField =
                FluidField.Get()
                    .SetLabelText("Input Min/Max")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .SetStyleAlignItems(Align.Center)
                            .AddChild(DesignUtils.NewFieldNameLabel("Min"))
                            .AddSpaceBlock()
                            .AddChild(inputMinFloatField)
                            .AddSpaceBlock(2)
                            .AddChild(DesignUtils.NewFieldNameLabel("Max"))
                            .AddSpaceBlock()
                            .AddChild(inputMaxFloatField)
                    );

            FloatField outputMinFloatField =
                DesignUtils.NewFloatField(propertyOutputMin)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The minimum value of the output range");

            FloatField outputMaxFloatField =
                DesignUtils.NewFloatField(propertyOutputMax)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The maximum value of the output range");

            FluidField outputMinMaxFluidField =
                FluidField.Get()
                    .SetLabelText("Output Min/Max")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .SetStyleAlignItems(Align.Center)
                            .AddChild(DesignUtils.NewFieldNameLabel("Min"))
                            .AddSpaceBlock()
                            .AddChild(outputMinFloatField)
                            .AddSpaceBlock(2)
                            .AddChild(DesignUtils.NewFieldNameLabel("Max"))
                            .AddSpaceBlock()
                            .AddChild(outputMaxFloatField)
                    );

            contentContainer
                .AddChild(inputMinMaxFluidField)
                .AddSpaceBlock()
                .AddChild(outputMinMaxFluidField);
        }
    }
}
