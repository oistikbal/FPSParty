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
    [CustomEditor(typeof(TimeSpanTransformer), true)]
    public class TimeSpanTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;
        
        private SerializedProperty propertyTimeSpanFormat { get; set; }
        
        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyTimeSpanFormat = serializedObject.FindProperty("TimeSpanFormat");
        }
        
        protected override void InitializeCustomInspector()
        {
            TextField timeSpanFormatTextField =
                DesignUtils.NewTextField(propertyTimeSpanFormat)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The format string to use for formatting the time span value.");

            FluidField timeSpanFormatFluidField =
                FluidField.Get()
                    .SetLabelText("TimeSpan Format")
                    .AddFieldContent(timeSpanFormatTextField);

            contentContainer
                .AddChild(timeSpanFormatFluidField);
        }
    }
}
