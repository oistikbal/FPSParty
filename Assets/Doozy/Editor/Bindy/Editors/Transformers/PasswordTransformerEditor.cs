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
    [CustomEditor(typeof(PasswordTransformer), true)]
    public class PasswordTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;
        
        private SerializedProperty propertyMaskCharacter { get; set; }
        private SerializedProperty propertyVisibleCharacters { get; set; }
        
        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyMaskCharacter = serializedObject.FindProperty("MaskCharacter");
            propertyVisibleCharacters = serializedObject.FindProperty("VisibleCharacters");
        }
        
        protected override void InitializeCustomInspector()
        {
            TextField maskCharacterTextField =
                DesignUtils.NewTextField(propertyMaskCharacter)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The character to use as a mask for the password");
            
            FluidField maskCharacterFluidField =
                FluidField.Get()
                    .SetLabelText("Mask Character")
                    .AddFieldContent(maskCharacterTextField);
            
            IntegerField visibleCharactersIntegerField =
                DesignUtils.NewIntegerField(propertyVisibleCharacters)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The number of characters to leave visible at the start of the original string");
            
            FluidField visibleCharactersFluidField =
                FluidField.Get()
                    .SetLabelText("Visible Characters")
                    .AddFieldContent(visibleCharactersIntegerField);
            
            contentContainer
                .AddChild(maskCharacterFluidField)
                .AddSpaceBlock()
                .AddChild(visibleCharactersFluidField);
        }
    }
}
