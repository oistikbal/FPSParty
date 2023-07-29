// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Bindy;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ObjectNames = Doozy.Runtime.Common.Utils.ObjectNames;
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Bindy.Editors
{
    [CustomEditor(typeof(ValueTransformer), true)]
    public class ValueTransformerEditor : UnityEditor.Editor
    {
        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }

        /// <summary>
        /// Set to TRUE if you're customizing the editor and you want to use your own custom editor
        /// <para/> This will inject the base editor fields into the root VisualElement
        /// <para/> Then you can add your custom editor fields to the contentContainer VisualElement
        /// </summary>
        protected virtual bool customEditor => false;

        /// <summary> Accent color for the editor </summary>
        protected virtual Color accentColor => EditorColors.Bindy.Color;

        /// <summary> Selectable color for the editor </summary>
        protected virtual EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Bindy.Color;

        protected Label descriptionLabel { get; private set; }
        protected VisualElement contentContainer { get; private set; }

        private FluidToggleSwitch enabledSwitch { get; set; }
        private SerializedProperty propertyEnabled { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            FindSerializedProperties();
            Initialize();
            Compose();
            return root;
        }

        protected virtual void FindSerializedProperties()
        {
            propertyEnabled = serializedObject.FindProperty("Enabled");
        }

        private void Initialize()
        {
            root =
                DesignUtils.editorRoot;

            string componentName =
                ObjectNames.NicifyVariableName
                (
                    target
                        .GetType().Name
                        .Replace("Transformer", string.Empty)
                );

            componentHeader =
                DesignUtils.editorComponentHeader
                    .SetIcon(EditorSpriteSheets.Bindy.Icons.Transformer)
                    .SetComponentNameText(componentName)
                    .SetComponentTypeText("Value Transformer")
                    .SetAccentColor(EditorColors.Bindy.Color);

            descriptionLabel =
                DesignUtils.NewFieldNameLabel(((ValueTransformer)target).description)
                    .SetStyleBackgroundColor(EditorColors.Default.FieldBackground)
                    .SetStyleBorderBottomLeftRadius(DesignUtils.k_FieldBorderRadius)
                    .SetStyleBorderBottomRightRadius(DesignUtils.k_FieldBorderRadius)
                    .SetStylePadding(DesignUtils.k_Spacing2X)
                    .SetWhiteSpace(WhiteSpace.Normal)
                    .SetStyleFontSize(10)
                    .SetStyleTop(-8)
                    .SetStyleMarginLeft(48)
                    .SetStyleMarginRight(4)
                    .SetStyleMarginBottom(DesignUtils.k_Spacing);

            descriptionLabel.text +=
                "\n\n" +
                "---- Note ----" +
                "\n" +
                "Any settings you change will affect all the components that use this transformer. " +
                "If you want to change the settings for a specific component, " +
                "then you need to create another transformer asset and reference that one instead.";

            contentContainer =
                new VisualElement()
                    .SetStyleMarginLeft(44);

            InitializeBaseInspector();
            InitializeCustomInspector();
        }

        /// <summary>
        /// Override this method to add your custom editor fields.
        /// <para/> All the custom editor fields should be added to the contentContainer VisualElement
        /// </summary>
        protected virtual void InitializeCustomInspector()
        {

        }

        protected virtual void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddSpaceBlock()
                .AddChild(descriptionLabel)
                .AddChild(contentContainer);
        }


        private void InitializeBaseInspector()
        {
            if (!customEditor)
            {
                //if we're not customizing the editor, then we just use the default inspector
                InspectorElement.FillDefaultInspector(contentContainer, serializedObject, this);
                return;
            }

            enabledSwitch =
                FluidToggleSwitch.Get()
                    .BindToProperty(propertyEnabled)
                    .SetLabelText("Enabled")
                    .SetTooltip("Specifies if the transformer is enabled or not. If disabled, the transformer will not run.")
                    .SetToggleAccentColor(selectableAccentColor);

            contentContainer
                .AddChild(enabledSwitch)
                .AddSpaceBlock(2);
        }
    }
}
