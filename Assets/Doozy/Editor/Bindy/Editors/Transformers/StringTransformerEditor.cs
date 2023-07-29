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
    [CustomEditor(typeof(StringTransformer), true)]
    public class StringTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;
        
        private SerializedProperty propertyStringFormat { get; set; }
        
        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyStringFormat = serializedObject.FindProperty("StringFormat");
        }
        
        protected override void InitializeCustomInspector()
        {
            TextField stringFormatTextField =
                DesignUtils.NewTextField(propertyStringFormat)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The string format to use when converting the value to a string");
            
            FluidField stringFormatFluidField =
                FluidField.Get()
                    .SetLabelText("String Format")
                    .AddFieldContent(stringFormatTextField);
            
            contentContainer
                .AddChild(stringFormatFluidField);
        }
    }
}
