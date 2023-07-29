// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Bindy.Transformers;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;

namespace Doozy.Editor.Bindy.Editors.Transformers
{
    [CustomEditor(typeof(UrlTransformer), true)]
    public class UrlTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;
        
        private SerializedProperty propertyParameters { get; set; }
        
        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyParameters = serializedObject.FindProperty("Parameters");
        }

        protected override void InitializeCustomInspector()
        {
            FluidListView parametersFluidListView =
                DesignUtils.NewStringListView
                (
                    propertyParameters,
                    "Parameters",
                    "List of parameters to add to the URL"
                );
            
            contentContainer
                .AddChild(parametersFluidListView);
        }
    }
}
