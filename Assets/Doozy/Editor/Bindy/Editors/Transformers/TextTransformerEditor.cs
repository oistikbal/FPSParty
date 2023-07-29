// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Bindy.Transformers;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Doozy.Editor.Bindy.Editors.Transformers
{
    [CustomEditor(typeof(TextTransformer), true)]
    public class TextTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;

        private SerializedProperty propertyMode { get; set; }
        private SerializedProperty propertyTrim { get; set; }
        private SerializedProperty propertyRemoveExtraSpaces { get; set; }
        private SerializedProperty propertyRemoveDiacritics { get; set; }
        private SerializedProperty propertyRemoveNonAlphanumeric { get; set; }
        private SerializedProperty propertyRemoveCharacters { get; set; }
        private SerializedProperty propertyCharactersToRemove { get; set; }
        private SerializedProperty propertyReverse { get; set; }
        private SerializedProperty propertyRemoveTabs { get; set; }
        private SerializedProperty propertyRemoveNewLines { get; set; }
        private SerializedProperty propertyRemoveLineBreaks { get; set; }
        private SerializedProperty propertyRemoveVerticalTabs { get; set; }
        private SerializedProperty propertyTruncate { get; set; }
        private SerializedProperty propertyMaxLength { get; set; }
        private SerializedProperty propertyTruncationIndicator { get; set; }

        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyMode = serializedObject.FindProperty("Mode");
            propertyTrim = serializedObject.FindProperty("Trim");
            propertyRemoveExtraSpaces = serializedObject.FindProperty("RemoveExtraSpaces");
            propertyRemoveTabs = serializedObject.FindProperty("RemoveTabs");
            propertyRemoveNewLines = serializedObject.FindProperty("RemoveNewLines");
            propertyRemoveLineBreaks = serializedObject.FindProperty("RemoveLineBreaks");
            propertyRemoveVerticalTabs = serializedObject.FindProperty("RemoveVerticalTabs");
            propertyRemoveDiacritics = serializedObject.FindProperty("RemoveDiacritics");
            propertyRemoveNonAlphanumeric = serializedObject.FindProperty("RemoveNonAlphanumeric");
            propertyRemoveCharacters = serializedObject.FindProperty("RemoveCharacters");
            propertyCharactersToRemove = serializedObject.FindProperty("CharactersToRemove");
            propertyReverse = serializedObject.FindProperty("Reverse");
            propertyTruncate = serializedObject.FindProperty("Truncate");
            propertyMaxLength = serializedObject.FindProperty("MaxLength");
            propertyTruncationIndicator = serializedObject.FindProperty("TruncationIndicator");
        }

        protected override void InitializeCustomInspector()
        {
            EnumField modeEnumField =
                DesignUtils.NewEnumField(propertyMode)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The mode in which the transformer will operate.");

            FluidField modeFluidField =
                FluidField.Get()
                    .SetLabelText("Mode")
                    .AddFieldContent(modeEnumField);

            FluidToggleCheckbox trimToggleCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Trim")
                    .BindToProperty(propertyTrim)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, the transformer will trim the text.");

            FluidToggleCheckbox removeExtraSpacesToggleCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Remove Extra Spaces")
                    .BindToProperty(propertyRemoveExtraSpaces)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, the transformer will remove extra spaces from the text.");

            FluidToggleCheckbox removeTabsToggleCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Remove Tabs")
                    .BindToProperty(propertyRemoveTabs)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, the transformer will remove tabs from the text.");

            FluidToggleCheckbox removeNewLinesToggleCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Remove New Lines")
                    .BindToProperty(propertyRemoveNewLines)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, the transformer will remove new lines from the text.");

            FluidToggleCheckbox removeLineBreaksToggleCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Remove Line Breaks")
                    .BindToProperty(propertyRemoveLineBreaks)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, the transformer will remove line breaks from the text.");

            FluidToggleCheckbox removeVerticalTabsToggleCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Remove Vertical Tabs")
                    .BindToProperty(propertyRemoveVerticalTabs)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, the transformer will remove vertical tabs from the text.");

            FluidToggleCheckbox removeDiacriticsToggleCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Remove Diacritics")
                    .BindToProperty(propertyRemoveDiacritics)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, the transformer will remove diacritics from the text.");

            FluidToggleCheckbox removeNonAlphanumericToggleCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Remove Non-Alphanumeric")
                    .BindToProperty(propertyRemoveNonAlphanumeric)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, the transformer will remove non-alphanumeric characters from the text.");

            TextField charactersToRemoveTextField =
                DesignUtils.NewTextField(propertyCharactersToRemove)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The characters that will be removed from the text.");

            charactersToRemoveTextField.SetEnabled(propertyRemoveCharacters.boolValue);

            FluidToggleCheckbox removeCharactersToggleCheckbox =
                FluidToggleCheckbox.Get()
                    .BindToProperty(propertyRemoveCharacters)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, the transformer will remove the characters specified in the Characters To Remove field.")
                    .SetOnClick(() => charactersToRemoveTextField.SetEnabled(propertyRemoveCharacters.boolValue));

            FluidField charactersToRemoveFluidField =
                FluidField.Get()
                    .SetLabelText("Characters To Remove")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .SetStyleAlignItems(Align.Center)
                            .AddChild(removeCharactersToggleCheckbox)
                            .AddSpaceBlock(2)
                            .AddChild(charactersToRemoveTextField)
                    );

            FluidToggleCheckbox reverseToggleCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Reverse")
                    .BindToProperty(propertyReverse)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, the transformer will reverse the text.");

            TextField maxLengthTextField =
                DesignUtils.NewTextField(propertyMaxLength)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The maximum length of the text if the Truncate option is enabled.");

            TextField truncationIndicatorTextField =
                DesignUtils.NewTextField(propertyTruncationIndicator)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text that will be added to the end of the text if the Truncate option is enabled.");

            FluidToggleCheckbox truncateToggleCheckbox =
                FluidToggleCheckbox.Get()
                    .BindToProperty(propertyTruncate)
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetTooltip("If TRUE, the transformer will truncate the text to the specified Max Length.");
            
            FluidField truncateFluidField =
                FluidField.Get()
                    .SetLabelText("Truncate")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .SetStyleAlignItems(Align.Center)
                            .AddChild(truncateToggleCheckbox)
                            .AddSpaceBlock(2)
                            .AddChild(DesignUtils.NewFieldNameLabel("Max Length"))
                            .AddSpaceBlock()
                            .AddChild(maxLengthTextField)
                            .AddSpaceBlock(2)
                            .AddChild(DesignUtils.NewFieldNameLabel("Truncation Indicator"))
                            .AddSpaceBlock()
                            .AddChild(truncationIndicatorTextField)
                    );
            
            contentContainer
                .AddChild(modeFluidField)
                .AddSpaceBlock()
                .AddChild(charactersToRemoveFluidField)
                .AddSpaceBlock()
                .AddChild(truncateFluidField)
                .AddSpaceBlock()
                .AddChild(trimToggleCheckbox)
                .AddSpaceBlock()
                .AddChild(removeExtraSpacesToggleCheckbox)
                .AddSpaceBlock()
                .AddChild(removeTabsToggleCheckbox)
                .AddSpaceBlock()
                .AddChild(removeNewLinesToggleCheckbox)
                .AddSpaceBlock()
                .AddChild(removeLineBreaksToggleCheckbox)
                .AddSpaceBlock()
                .AddChild(removeVerticalTabsToggleCheckbox)
                .AddSpaceBlock()
                .AddChild(removeDiacriticsToggleCheckbox)
                .AddSpaceBlock()
                .AddChild(removeNonAlphanumericToggleCheckbox)
                .AddSpaceBlock()
                .AddChild(reverseToggleCheckbox);
        }
    }
}
