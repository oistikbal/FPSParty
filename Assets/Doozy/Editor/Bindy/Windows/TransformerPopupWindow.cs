// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIElements;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Bindy.Windows
{
    public class TransformerPopupWindow : EditorWindow
    {
        private const string k_WindowTitle = "Bindy Transformer";

        public static TransformerPopupWindow Open() =>
            GetWindow<TransformerPopupWindow>(true, k_WindowTitle, true);

        // /// <summary>
        // /// Create an asset for each transformer type
        // /// </summary>
        // [MenuItem("Tools/Bindy/Create Transformer Assets", false, 0)]
        // private static void CreateTransformers()
        // {
        //     //get all the types that inherit from ValueTransformer
        //     IEnumerable<Type> types = ReflectionUtils.GetDerivedTypes(typeof(ValueTransformer));
        //     foreach (Type type in types)
        //     {
        //         ScriptableObject asset = CreateInstance(type);
        //         string fileName = type.Name.Replace("Transformer", "");
        //         string path = $"{RuntimePath.path}/Bindy/Data/{fileName}.asset";
        //         AssetDatabase.CreateAsset(asset, path);
        //     }
        // }
        
        public Object asset { get; private set; }

        protected VisualElement root => rootVisualElement;
        private VisualElement assetEditorContainer { get; set; }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public TransformerPopupWindow LoadAsset(Object target)
        {
            assetEditorContainer.RecycleAndClear();
            asset = target;
            if (asset == null)
            {
                EditorUtility.DisplayDialog
                (
                    "Can't load asset",
                    "The asset is null",
                    "Ok"
                );
                return this;
            }
            var editor = UnityEditor.Editor.CreateEditor(asset);
            VisualElement editorRoot = editor.CreateInspectorGUI();
            editorRoot.Bind(editor.serializedObject);
            assetEditorContainer.AddChild(editorRoot);

            editorRoot.SetStylePadding(DesignUtils.k_Spacing2X);

            return this;
        }

        private void CreateGUI()
        {
            assetEditorContainer = new VisualElement();
            root
                .RecycleAndClear()
                .AddChild(assetEditorContainer);
        }

        private void OnEnable()
        {
            minSize = new Vector2(500, 500);
            
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            Close();
        }
    }
}
