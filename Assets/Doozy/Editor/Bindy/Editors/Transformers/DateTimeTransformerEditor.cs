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
    [CustomEditor(typeof(DateTimeTransformer), true)]
    public class DateTimeTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;

        private SerializedProperty propertyFormatType { get; set; }
        private SerializedProperty propertyCustomDateTimeFormat { get; set; }
        private SerializedProperty propertyCulture { get; set; }
        private SerializedProperty propertyCultureName { get; set; }


        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyFormatType = serializedObject.FindProperty("FormatType");
            propertyCustomDateTimeFormat = serializedObject.FindProperty("CustomDateTimeFormat");
            propertyCulture = serializedObject.FindProperty("Culture");
            propertyCultureName = serializedObject.FindProperty("CultureName");
        }

        protected override void InitializeCustomInspector()
        {
            EnumField formatTypeEnumField =
                DesignUtils.NewEnumField(propertyFormatType)
                    .SetStyleFlexGrow(1.2f)
                    .SetTooltip("The format type to use when converting the DateTime value to a string");

            FluidField formatTypeFluidField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetLabelText("Format Type")
                    .AddFieldContent(formatTypeEnumField);

            TextField customDateTimeFormatField =
                DesignUtils.NewTextField(propertyCustomDateTimeFormat)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The custom format to use when converting the DateTime value to a string");

            FluidField customDateTimeFormatFluidField =
                FluidField.Get()
                    .SetLabelText("Custom Format")
                    .AddFieldContent(customDateTimeFormatField);

            customDateTimeFormatFluidField.SetEnabled((DateTimeTransformer.DateTimeFormatType)propertyFormatType.enumValueIndex == DateTimeTransformer.DateTimeFormatType.Custom);
            formatTypeEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                customDateTimeFormatFluidField.SetEnabled((DateTimeTransformer.DateTimeFormatType)evt.newValue == DateTimeTransformer.DateTimeFormatType.Custom);
            });


            EnumField cultureEnumField =
                DesignUtils.NewEnumField(propertyCulture)
                    .SetStyleFlexGrow(1.2f)
                    .SetTooltip("The culture to use when converting the DateTime value to a string");

            FluidField cultureFluidField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetLabelText("Culture")
                    .AddFieldContent(cultureEnumField);

            TextField cultureNameField =
                DesignUtils.NewTextField(propertyCultureName)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The culture name to use when converting the DateTime value to a string");

            FluidField cultureNameFluidField =
                FluidField.Get()
                    .SetLabelText("Culture Name")
                    .AddFieldContent(cultureNameField);

            cultureNameFluidField.SetEnabled((DateTimeTransformer.CultureType)propertyCulture.enumValueIndex == DateTimeTransformer.CultureType.Specific);
            cultureEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                cultureNameFluidField.SetEnabled((DateTimeTransformer.CultureType)evt.newValue == DateTimeTransformer.CultureType.Specific);
            });

            contentContainer
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(formatTypeFluidField)
                        .AddSpaceBlock()
                        .AddChild(customDateTimeFormatFluidField)
                )
                .AddSpaceBlock()
                .AddChild(
                    DesignUtils.row
                        .AddChild(cultureFluidField)
                        .AddSpaceBlock()
                        .AddChild(cultureNameFluidField)
                );


        }
    }
}
