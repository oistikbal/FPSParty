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
    [CustomEditor(typeof(TimeTransformer), true)]
    public class TimeTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;
        
        private SerializedProperty propertyTimeFormat { get; set; }
        
        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyTimeFormat = serializedObject.FindProperty("TimeFormat");
        }
        
        protected override void InitializeCustomInspector()
        {
            TextField timeFormatTextField =
                DesignUtils.NewTextField(propertyTimeFormat)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The format string to use for formatting the time value.");
            
            FluidField timeFormatFluidField =
                FluidField.Get()
                    .SetLabelText("Time Format")
                    .AddFieldContent(timeFormatTextField);
            
            contentContainer
                .AddChild(timeFormatFluidField);
        }
    }
}
