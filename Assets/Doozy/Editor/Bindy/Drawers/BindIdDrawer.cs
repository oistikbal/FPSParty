// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Bindy.ScriptableObjects;
using Doozy.Editor.Bindy.Windows;
using Doozy.Editor.Common.Drawers;
using Doozy.Editor.EditorUI;
using Doozy.Runtime.Bindy;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Bindy.Drawers
{
    [CustomPropertyDrawer(typeof(BindId), true)]
    public class BindIdDrawer: PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property) =>
            CategoryNameIdUtils.CreateDrawer
            (
                property,
                () => BindIdDatabase.instance.database.GetCategories(),
                targetCategory => BindIdDatabase.instance.database.GetNames(targetCategory),
                EditorSpriteSheets.EditorUI.Icons.GenericDatabase,
                BindsDatabaseWindow.Open,
                "Open Binds Database Window",
                BindIdDatabase.instance,
                EditorSelectableColors.Bindy.Color
            );
    }
}
