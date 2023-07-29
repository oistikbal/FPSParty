// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Bindy.Transformers;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine.UIElements;

namespace Doozy.Editor.Bindy.Editors.Transformers
{
    [CustomEditor(typeof(BooleanTransformer), true)]
    public class BooleanTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;

        private SerializedProperty propertyTrueString { get; set; }
        private SerializedProperty propertyFalseString { get; set; }

        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyTrueString = serializedObject.FindProperty("TrueString");
            propertyFalseString = serializedObject.FindProperty("FalseString");
        }

        protected override void InitializeCustomInspector()
        {
            TextField trueTextField =
                DesignUtils.NewTextField(propertyTrueString)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The string value to return when the boolean value is true");

            FluidField trueFluidField =
                FluidField.Get()
                    .SetLabelText("True")
                    .AddFieldContent(trueTextField);

            TextField falseTextField =
                DesignUtils.NewTextField(propertyFalseString)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The string value to return when the boolean value is false");

            FluidField falseFluidField =
                FluidField.Get()
                    .SetLabelText("False")
                    .AddFieldContent(falseTextField);

            contentContainer
                .AddChild(trueFluidField)
                .AddSpaceBlock()
                .AddChild(falseFluidField);
        }
    }
}
