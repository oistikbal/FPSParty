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
    [CustomEditor(typeof(FileSizeTransformer), true)]
    public class FileSizeTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;

        private SerializedProperty propertyUnitSize { get; set; }
        private SerializedProperty propertyUnitSuffixes { get; set; }

        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyUnitSize = serializedObject.FindProperty("UnitSize");
            propertyUnitSuffixes = serializedObject.FindProperty("UnitSuffixes");
        }

        protected override void InitializeCustomInspector()
        {
            UnityEngine.UIElements.IntegerField unitSizeIntegerField =
                DesignUtils.NewIntegerField(propertyUnitSize)
                    .SetStyleFlexGrow(1)
                    .SetTooltip
                    (
                        "The size of the unit (in bytes) that will be used to calculate the file size." +
                        "The base unit size (e.g. 1 KB = 1024 bytes)."
                    );

            FluidField unitSizeFluidField =
                FluidField.Get()
                    .SetLabelText("Unit Size")
                    .AddFieldContent(unitSizeIntegerField);

            FluidListView unitSuffixesFluidListView =
                DesignUtils.NewStringListView
                (
                    propertyUnitSuffixes,
                    "Unit Suffixes",
                    "The available units and their suffixes."
                );
            
            contentContainer
                .AddChild(unitSizeFluidField)
                .AddSpaceBlock(2)
                .AddChild(unitSuffixesFluidListView);
        }
    }
}
