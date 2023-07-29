// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Bindy;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Bindy.Editors
{
    [CustomEditor(typeof(Binder), true)]
    public class BinderEditor : UnityEditor.Editor
    {
        private Binder castedTarget => (Binder)target;

        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }

        private PropertyField bindIdPropertyField { get; set; }
        private FluidField bindIdFluidField { get; set; }
        private VisualElement bindablesContainer { get; set; }
        private FluidButton addBindableButton { get; set; }

        private SerializedProperty propertyBindId { get; set; }
        private SerializedProperty propertyBindables { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            FindSerializedProperties();
            Initialize();
            Compose();
            return root;
        }

        private void FindSerializedProperties()
        {
            propertyBindId = serializedObject.FindProperty("BindId");
            propertyBindables = serializedObject.FindProperty("Bindables");
        }

        private void Initialize()
        {
            root = DesignUtils.editorRoot;
            componentHeader =
                DesignUtils.editorComponentHeader
                    .SetIcon(EditorSpriteSheets.Bindy.Icons.Binder)
                    .SetAccentColor(EditorColors.Bindy.Color)
                    .SetComponentNameText("Binder")
                    .AddManualButton()
                    .AddApiButton()
                    .AddYouTubeButton();

            addBindableButton =
                FluidButton.Get()
                    .SetLabelText("Add Bindable")
                    .SetTooltip("Add a new Bindable to this Binder")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Plus)
                    .SetElementSize(ElementSize.Small)
                    .SetAccentColor(EditorSelectableColors.Bindy.Color)
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetOnClick(() =>
                    {
                        Undo.RecordObject(castedTarget, "Add Bindable");
                        castedTarget.bindables.Insert(0, new Bindable());
                        UpdateBindables();
                    });

            bindablesContainer =
                new VisualElement();

            InitializeBindId();
            InitializeBindables();
            
            //check for changes every 500ms
            //we need to do this because the Bindables list is not serialized
            //and we can't use the SerializedObject to detect changes when an undo/redo is performed
            //so we need to check for changes manually
            //this is not the best solution, but it works for now
            //thank you Unity for not providing a better solution for this problem :(
            root.schedule.Execute(() =>
                {
                    if (!bindablesChanged) return;
                    UpdateBindables();
                })
                .Every(500)
                .StartingIn(500);
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddSpaceBlock()
                .AddChild(bindIdFluidField)
                .AddSpaceBlock(2)
                .AddChild
                (
                    DesignUtils.row
                        .AddFlexibleSpace()
                        .AddChild(addBindableButton)
                )
                .AddSpaceBlock(2)
                .AddChild(bindablesContainer)
                .AddEndOfLineSpace();
        }

        private void InitializeBindId()
        {
            bindIdPropertyField = DesignUtils.NewPropertyField(propertyBindId);

            bindIdFluidField =
                FluidField.Get()
                    .SetLabelText("Bind Id")
                    .SetTooltip("The BindId that this Binder will be connect to or use to create a new Bind object")
                    .AddFieldContent(bindIdPropertyField);

        }

        private int visibleBindablesCount { get; set; }
        private int numberOfBindables => propertyBindables.arraySize;
        private bool bindablesChanged => visibleBindablesCount != numberOfBindables;

        private void InitializeBindables()
        {
            visibleBindablesCount = 0;
            for (int i = 0; i < propertyBindables.arraySize; i++)
            {
                visibleBindablesCount++;
                var property = propertyBindables.GetArrayElementAtIndex(i);
                var propertyField = DesignUtils.NewPropertyField(property);
                var container =
                    new VisualElement()
                        .SetStyleBorderRadius(DesignUtils.k_FieldBorderRadius)
                        .SetStyleBorderRadius(DesignUtils.k_Spacing);

                int index = i;
                var removeButton =
                    FluidButton.Get()
                        .SetTooltip("Remove Bindable")
                        .SetElementSize(ElementSize.Tiny)
                        .SetIcon(EditorSpriteSheets.EditorUI.Icons.Close)
                        .SetButtonStyle(ButtonStyle.Contained)
                        .SetAccentColor(EditorSelectableColors.Default.Remove)
                        .SetOnClick(() =>
                        {
                            propertyBindables.DeleteArrayElementAtIndex(index);
                            serializedObject.ApplyModifiedProperties();
                            UpdateBindables();
                        });

                var rowToolbar =
                    new VisualElement()
                        .SetStyleFlexDirection(FlexDirection.Row)
                        .SetStylePaddingLeft(DesignUtils.k_Spacing)
                        .SetStylePaddingRight(DesignUtils.k_Spacing)
                        .AddFlexibleSpace()
                        .AddChild(removeButton);

                container
                    .AddChild(rowToolbar)
                    .AddChild(propertyField);

                bindablesContainer
                    .AddSpaceBlock()
                    .AddChild(DesignUtils.dividerHorizontal)
                    .AddChild(container)
                    .AddChild(DesignUtils.dividerHorizontal)
                    .AddSpaceBlock(2)
                    ;

                //do not add space to the last element
                if (i < propertyBindables.arraySize - 1)
                    bindablesContainer
                        .AddSpaceBlock(2);
            }
        }

        private void UpdateBindables()
        {
            serializedObject.UpdateIfRequiredOrScript();
            bindablesContainer.Clear();
            InitializeBindables();
            bindablesContainer.Bind(serializedObject);
        }

    }
}
