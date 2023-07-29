// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Bindy.Windows;
using Doozy.Editor.Common.Extensions;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Bindy;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Bindy.Drawers
{
    [CustomPropertyDrawer(typeof(Bindable), true)]
    public class BindableDrawer : PropertyDrawer
    {
        public const string TOOLBAR_CONTAINER_NAME = "ToolbarContainer";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public static IEnumerable<Texture2D> bindableIcon => EditorSpriteSheets.Bindy.Icons.Bindable;

        public static IEnumerable<Texture2D> senderIcon => EditorSpriteSheets.Bindy.Icons.Sender;
        public static Color senderColor => EditorColors.Bindy.Sender;
        public static EditorSelectableColorInfo senderSelectableColor => EditorSelectableColors.Bindy.Sender;

        public static IEnumerable<Texture2D> bidirectionalIcon => EditorSpriteSheets.Bindy.Icons.Bidirectional;
        public static Color bidirectionalColor => EditorColors.Bindy.Bidirectional;
        public static EditorSelectableColorInfo bidirectionalSelectableColor => EditorSelectableColors.Bindy.Bidirectional;

        public static IEnumerable<Texture2D> receiverIcon => EditorSpriteSheets.Bindy.Icons.Receiver;
        public static Color receiverColor => EditorColors.Bindy.Receiver;
        public static EditorSelectableColorInfo receiverSelectableColor => EditorSelectableColors.Bindy.Receiver;

        public static Color GetConnectionTypeColor(ConnectionType connectionType)
        {
            switch (connectionType)
            {
                case ConnectionType.Sender:
                    return senderColor;
                case ConnectionType.Bidirectional:
                    return bidirectionalColor;
                case ConnectionType.Receiver:
                    return receiverColor;
                default:
                    throw new ArgumentOutOfRangeException(nameof(connectionType), connectionType, null);
            }
        }

        public static EditorSelectableColorInfo GetConnectionTypeSelectableColor(ConnectionType connectionType)
        {
            switch (connectionType)
            {
                case ConnectionType.Sender:
                    return senderSelectableColor;
                case ConnectionType.Bidirectional:
                    return bidirectionalSelectableColor;
                case ConnectionType.Receiver:
                    return receiverSelectableColor;
                default:
                    throw new ArgumentOutOfRangeException(nameof(connectionType), connectionType, null);
            }
        }

        public static IEnumerable<Texture2D> GetConnectionTypeIcon(ConnectionType connectionType)
        {
            switch (connectionType)
            {
                case ConnectionType.Sender:
                    return senderIcon;
                case ConnectionType.Bidirectional:
                    return bidirectionalIcon;
                case ConnectionType.Receiver:
                    return receiverIcon;
                default:
                    throw new ArgumentOutOfRangeException(nameof(connectionType), connectionType, null);
            }
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var target = property.GetTargetObjectOfProperty() as Bindable;
            var drawer = new VisualElement();

            var propertyConnectionType = property.FindPropertyRelative("ConnectionType");
            var propertyBindyValue = property.FindPropertyRelative("BindyValue");
            var propertyOnBindBehavior = property.FindPropertyRelative("OnBindBehavior");
            var propertyTransformer = property.FindPropertyRelative("Transformer");
            var propertyTicker = property.FindPropertyRelative("Ticker");

            var connectionTypeEnumField = DesignUtils.NewEnumField(propertyConnectionType, true);
            var bindyValuePropertyField = DesignUtils.NewPropertyField(propertyBindyValue);

            FluidComponentHeader header =
                FluidComponentHeader.Get()
                    .SetComponentTypeText("Bindable")
                    .SetSecondaryIcon(bindableIcon.ToList())
                    .SetElementSize(ElementSize.Small);

            UpdateHeaderIcon((ConnectionType)propertyConnectionType.enumValueIndex, false);

            void UpdateHeaderIcon(ConnectionType connectionType, bool animate = true)
            {
                header
                    .SetIcon(GetConnectionTypeIcon(connectionType).ToList())
                    .SetAccentColor(GetConnectionTypeColor(connectionType));

                if (!animate) return;
                header.iconReaction?.Play();
                header.rotatedIconReaction?.Play();
                header.secondaryIconReaction?.Play();
            }

            var contentContainer =
                    new VisualElement()
                        .SetName("ContentContainer")
                        .SetStyleMarginLeft(34);

            var toolbarContainer =
                new VisualElement()
                    .SetName(TOOLBAR_CONTAINER_NAME)
                    .SetStyleFlexDirection(FlexDirection.Row)
                    .SetStyleFlexShrink(0);

            var transformerContainer =
                new VisualElement()
                    .SetStyleFlexDirection(FlexDirection.Row)
                    .SetStyleMarginBottom(DesignUtils.k_Spacing)
                    .SetStyleDisplay((ConnectionType)propertyConnectionType.enumValueIndex != ConnectionType.Sender ? DisplayStyle.Flex : DisplayStyle.None);

            var tickerContainer =
                DesignUtils.fieldContainer
                    .SetStyleMarginBottom(DesignUtils.k_Spacing)
                    .SetStyleDisplay((ConnectionType)propertyConnectionType.enumValueIndex != ConnectionType.Receiver ? DisplayStyle.Flex : DisplayStyle.None);

            var transformerObjectField =
                DesignUtils.NewObjectField(propertyTransformer, typeof(ValueTransformer))
                    .SetStyleFlexGrow(1);

            var openTransformerPopupButton =
                FluidButton.Get()
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Settings)
                    .SetLabelText("Settings")
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetElementSize(ElementSize.Small)
                    .SetStyleFlexGrow(0)
                    .SetStyleFlexShrink(0)
                    .SetOnClick(() =>
                    {
                        TransformerPopupWindow
                            .Open()
                            .LoadAsset(propertyTransformer.objectReferenceValue);
                    });

            var transformerFluidField =
                FluidField.Get()
                    .SetLabelText("Value Transformer")
                    .SetTooltip("Transformer applied before the value is set (when receiving a value)")
                    .SetElementSize(ElementSize.Small)
                    .SetStyleFlexGrow(1)
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .SetStyleAlignItems(Align.FlexEnd)
                            .AddChild(transformerObjectField)
                            .AddSpaceBlock(2)
                            .AddChild(openTransformerPopupButton)
                    );

            openTransformerPopupButton.SetEnabled(propertyTransformer.objectReferenceValue != null);

            transformerObjectField.RegisterValueChangedCallback(evt =>
            {
                if (evt == null) return;
                openTransformerPopupButton.SetEnabled(evt.newValue != null);
            });

            var tickerPropertyField =
                DesignUtils.NewPropertyField(propertyTicker);

            var connectionToggleGroup =
                FluidToggleGroup.Get()
                    .SetControlMode(FluidToggleGroup.ControlMode.Passive);

            var senderTab =
                GetConnectionTypeTab
                (
                    ConnectionType.Sender,
                    "One Way Connection\n\n" +
                    "This Bindable will send updates to the Bind when its value changes. " +
                    "Does not receive updates from the Bind.\n\n" +
                    "Use this option when you only want to send updates to the Bind."
                );

            var bidirectionalTab =
                GetConnectionTypeTab
                (
                    ConnectionType.Bidirectional,
                    "Two Way Connection\n\n" +
                    "This Bindable will send updates to the Bind when its value changes. " +
                    "Also receives updates from the Bind.\n\n" +
                    "Use this option when you want to send and receive updates to and from the Bind."
                );

            var receiverTab =
                GetConnectionTypeTab
                (
                    ConnectionType.Receiver,
                    "One Way Connection\n\n" +
                    "This Bindable will receive updates from the Bind. " +
                    "Does not send updates to the Bind.\n\n" +
                    "Use this option when you only want to receive updates from the Bind."
                );

            FluidTab GetConnectionTypeTab(ConnectionType connectionType, string tooltipText)
            {
                bool isOn = propertyConnectionType.enumValueIndex == (int)connectionType;

                FluidTab tab =
                    FluidTab.Get()
                        .SetLabelText(connectionType.ToString())
                        .SetTooltip(tooltipText)
                        .SetElementSize(ElementSize.Normal)
                        .SetTabPosition(TabPosition.FloatingTab)
                        .ButtonSetAccentColor(GetConnectionTypeSelectableColor(connectionType))
                        .IndicatorSetEnabledColor(GetConnectionTypeColor(connectionType))
                        .SetIndicatorPosition(IndicatorPosition.Top)
                        .ButtonSetIsOn(isOn)
                        .IndicatorToggle(isOn)
                        .AddToToggleGroup(connectionToggleGroup);

                tab.ButtonSetOnValueChanged(evt =>
                {
                    tab.IndicatorToggle(evt.newValue);
                    if (!evt.newValue) return;
                    propertyConnectionType.enumValueIndex = (int)connectionType;
                    property.serializedObject.ApplyModifiedProperties();
                    drawer.schedule.Execute(() => UpdateHeaderIcon(connectionType));
                });

                return tab;
            }

            connectionToggleGroup.SetControlMode(FluidToggleGroup.ControlMode.OneToggleOnEnforced);

            connectionTypeEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                var newValue = (ConnectionType)evt.newValue;
                switch (newValue)
                {
                    case ConnectionType.Sender:
                        senderTab.ButtonSetIsOn(true);
                        senderTab.IndicatorToggle(true);
                        bidirectionalTab.IndicatorToggle(false);
                        receiverTab.IndicatorToggle(false);
                        transformerContainer.SetStyleDisplay(DisplayStyle.None);
                        tickerContainer.SetStyleDisplay(DisplayStyle.Flex);
                        break;
                    case ConnectionType.Bidirectional:
                        bidirectionalTab.ButtonSetIsOn(true);
                        bidirectionalTab.IndicatorToggle(true);
                        senderTab.IndicatorToggle(false);
                        receiverTab.IndicatorToggle(false);
                        transformerContainer.SetStyleDisplay(DisplayStyle.Flex);
                        tickerContainer.SetStyleDisplay(DisplayStyle.Flex);
                        break;
                    case ConnectionType.Receiver:
                        receiverTab.ButtonSetIsOn(true);
                        receiverTab.IndicatorToggle(true);
                        senderTab.IndicatorToggle(false);
                        bidirectionalTab.IndicatorToggle(false);
                        transformerContainer.SetStyleDisplay(DisplayStyle.Flex);
                        tickerContainer.SetStyleDisplay(DisplayStyle.None);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });

            var onBindBehaviorEnumField =
                DesignUtils.NewEnumField(propertyOnBindBehavior)
                    .SetStyleFlexGrow(1)
                    .SetTooltip
                    (
                        "What happens when the Bindable is bound to a Bind.\n\n" +
                        "Do Nothing\nNothing happens. The Bindable does not send or receive any value.\n\n" +
                        "Set Value\nThe Bindable sends its value to the Bind.\n\n" +
                        "Get Value\nThe Bindable receives the value from the Bind, by checking the last value sent by the Bind."
                    );

            var onBindBehaviorFluidField =
                FluidField.Get()
                    .SetLabelText("On Bind Behavior")
                    .SetElementSize(ElementSize.Small)
                    .SetStyleFlexShrink(0)
                    .SetStyleWidth(110)
                    .AddFieldContent(onBindBehaviorEnumField);

            header
                .AddElement
                (
                    DesignUtils.row
                        .SetStyleFlexGrow(0)
                        .SetStyleFlexShrink(0)
                        .AddChild(connectionTypeEnumField)
                        .AddChild(senderTab)
                        .AddSpace(2)
                        .AddChild(bidirectionalTab)
                        .AddSpace(2)
                        .AddChild(receiverTab)
                );

            transformerContainer
                .AddChild(transformerFluidField);

            tickerContainer
                .AddChild(tickerPropertyField);

            drawer
                .SetStyleBorderRadius(DesignUtils.k_FieldBorderRadius)
                .SetStylePadding(DesignUtils.k_Spacing);

            return
                drawer
                    .AddChild
                    (
                        DesignUtils.row
                            .AddChild(header)
                            .AddChild(toolbarContainer)
                    )
                    .AddSpace(2)
                    .AddChild
                    (
                        contentContainer
                            .AddChild
                            (
                                DesignUtils.row
                                    .AddChild(bindyValuePropertyField)
                                    .AddSpaceBlock()
                                    .AddChild(onBindBehaviorFluidField)
                            )
                            .AddSpaceBlock()
                            .AddChild(tickerContainer)
                            .AddChild(transformerContainer)
                    );
        }
    }
}
