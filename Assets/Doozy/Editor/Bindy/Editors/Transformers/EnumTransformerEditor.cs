// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Runtime.Bindy.Transformers;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;

namespace Doozy.Editor.Bindy.Editors.Transformers
{
    [CustomEditor(typeof(EnumTransformer), true)]
    public class EnumTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;

        private SerializedProperty propertyUseEnumName { get; set; }

        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyUseEnumName = serializedObject.FindProperty("UseEnumName");
        }

        protected override void InitializeCustomInspector()
        {
            FluidToggleCheckbox useEnumNameToggle =
                FluidToggleCheckbox.Get()
                    .BindToProperty(propertyUseEnumName)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetLabelText("Use Enum Name")
                    .SetTooltip
                    (
                        "If enabled, the enum name will be used as the string value. " +
                        "If disabled, the enum value will be used as the string value."
                    );

            FluidField optionsField =
                FluidField.Get()
                    .SetLabelText("Options")
                    .AddFieldContent(useEnumNameToggle);

            contentContainer
                .AddChild(optionsField);
        }
    }
}
