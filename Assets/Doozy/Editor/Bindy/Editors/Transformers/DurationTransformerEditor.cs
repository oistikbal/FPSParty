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
    [CustomEditor(typeof(DurationTransformer), true)]
    public class DurationTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;

        private SerializedProperty propertyDurationFormat { get; set; }

        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyDurationFormat = serializedObject.FindProperty("DurationFormat");
        }

        protected override void InitializeCustomInspector()
        {
            TextField durationFormatTextField =
                DesignUtils.NewTextField(propertyDurationFormat)
                    .SetStyleFlexGrow(1)
                    .SetTooltip
                    (
                        "The format of the duration string. " +
                        "The format is a string that contains zero or more custom format items, which are indicated by braces."
                    );

            FluidField durationFormatFluidField =
                FluidField.Get()
                    .SetLabelText("Duration Format")
                    .AddFieldContent(durationFormatTextField);

            contentContainer
                .AddChild(durationFormatFluidField);
        }
    }
}
