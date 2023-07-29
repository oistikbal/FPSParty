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
    [CustomEditor(typeof(PhoneNumberTransformer), true)]
    public class PhoneNumberTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;
        
        private SerializedProperty propertyLocalPattern { get; set; }
        private SerializedProperty propertyInternationalPattern { get; set; }
        
        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyLocalPattern = serializedObject.FindProperty("Pattern").FindPropertyRelative("Local");
            propertyInternationalPattern = serializedObject.FindProperty("Pattern").FindPropertyRelative("International");
        }
        
        protected override void InitializeCustomInspector()
        {
            TextField localPatternTextField =
                DesignUtils.NewTextField(propertyLocalPattern)
                    .SetStyleFlexGrow(1);
            
            FluidField localPatternFluidField =
                FluidField.Get()
                    .SetLabelText("Pattern")
                    .SetTooltip("The pattern used to format a local phone number")
                    .AddFieldContent(localPatternTextField);
            
            TextField internationalPatternTextField =
                DesignUtils.NewTextField(propertyInternationalPattern)
                    .SetStyleFlexGrow(1);
            
            FluidField internationalPatternFluidField =
                FluidField.Get()
                    .SetLabelText("Pattern")
                    .SetTooltip("The pattern used to format an international phone number")
                    .AddFieldContent(internationalPatternTextField);
            
         
            contentContainer
                .AddChild(localPatternFluidField)
                .AddSpaceBlock()
                .AddChild(internationalPatternFluidField);
        }
    }
}
