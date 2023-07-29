// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.Bindy.Automation.Generators;
using Doozy.Editor.Bindy.ScriptableObjects;
using Doozy.Editor.Common.Layouts;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.Interfaces;
using Doozy.Runtime.Common;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Doozy.Editor.Bindy.Layouts
{
    public sealed class BindsDatabaseWindowLayout : CategoryNameGroupWindowLayout, IDashboardDatabaseWindowLayout
    {
        public int order => 0;
        
        public override string layoutName => "Binds";
        public override List<Texture2D> animatedIconTextures => EditorSpriteSheets.Bindy.Icons.BindyDatabase;
        public override Color accentColor => EditorColors.Bindy.Color;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Bindy.Color;

        protected override Object targetObject => BindIdDatabase.instance;
        protected override UnityAction onUpdateCallback => BindIdDatabase.instance.onUpdateCallback; 
        protected override CategoryNameGroup<CategoryNameItem> database => BindIdDatabase.instance.database;
        protected override string groupTypeName => "Bind";

        protected override Func<string, List<string>, bool> exportDatabaseHandler => BindIdDatabase.instance.ExportRoamingDatabase;
        protected override Func<List<ScriptableObject>, bool> importDatabaseHandler => BindIdDatabase.instance.ImportRoamingDatabases;
        protected override string roamingDatabaseTypeName => nameof(BindIdRoamingDatabase);

        protected override UnityAction runEnumGenerator => () => BindIdExtensionGenerator.Run(true, false, true);
        
        public BindsDatabaseWindowLayout()
        {
            AddHeader("Binds Database", "Bind Ids", animatedIconTextures);
            sideMenu
                .SetMenuLevel(FluidSideMenu.MenuLevel.Level_2)
                .IsCollapsable(false)
                .SetAccentColor(selectableAccentColor);
            Initialize();
        }
    }
}
