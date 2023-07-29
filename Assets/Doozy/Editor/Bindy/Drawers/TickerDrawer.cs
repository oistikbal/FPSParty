// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.Common.Extensions;
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

namespace Doozy.Editor.Bindy.Drawers
{
    [CustomPropertyDrawer(typeof(Ticker), true)]
    public class TickerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var target = property.GetTargetObjectOfProperty() as Ticker;
            var drawer = new VisualElement();

            var propertyTickerMode = property.FindPropertyRelative("TickerMode");
            var propertyFrameInterval = property.FindPropertyRelative("FrameInterval");
            var propertyTimeInterval = property.FindPropertyRelative("TimeInterval");
            var propertyMaxTicksPerSecond = property.FindPropertyRelative("MaxTicksPerSecond");
            var propertyFrames = property.FindPropertyRelative("Frames");
            var propertySeconds = property.FindPropertyRelative("Seconds");

            var tickerModeEnumField = DesignUtils.NewEnumField(propertyTickerMode, true);
            var frameIntervalEnumField = DesignUtils.NewEnumField(propertyFrameInterval, true);
            var timeIntervalEnumField = DesignUtils.NewEnumField(propertyTimeInterval, true);

            Color tabColor = EditorColors.Bindy.Ticker;
            EditorSelectableColorInfo tabSelectableColor = EditorSelectableColors.Bindy.Ticker;

            var realtimeAnimatedContainer = new FluidAnimatedContainer("RealTime Container", true).Hide(false);
            var frameAnimatedContainer = new FluidAnimatedContainer("Frame Interval Container", true).Hide(false);
            var timeAnimatedContainer = new FluidAnimatedContainer("Time Interval Container", true).Hide(false);

            drawer.schedule.Execute(() =>
            {
                var tickerMode = (Ticker.Mode)propertyTickerMode.enumValueIndex;
                realtimeAnimatedContainer.Toggle(tickerMode == Ticker.Mode.RealTime, false);
                frameAnimatedContainer.Toggle(tickerMode == Ticker.Mode.FrameBased, false);
                timeAnimatedContainer.Toggle(tickerMode == Ticker.Mode.TimeBased, false);
            });

            #region Ticker Mode

            var tickerModeToggleGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.Passive);
            var realtimeTab = GetTickerModeTab(Ticker.Mode.RealTime, "RealTime", "Refresh every frame, regardless of the frame rate").SetTabPosition(TabPosition.TabOnLeft);
            var frameIntervalTab = GetTickerModeTab(Ticker.Mode.FrameBased, "Frame Interval", "Refresh every X frames").SetTabPosition(TabPosition.TabOnRight);
            var timeIntervalTab = GetTickerModeTab(Ticker.Mode.TimeBased, "Time Interval", "Refresh every X seconds").SetTabPosition(TabPosition.TabOnRight);
            tickerModeToggleGroup.SetControlMode(FluidToggleGroup.ControlMode.OneToggleOnEnforced);

            tickerModeEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                realtimeTab.ButtonSetIsOn((Ticker.Mode)evt.newValue == Ticker.Mode.RealTime, false);
                frameIntervalTab.ButtonSetIsOn((Ticker.Mode)evt.newValue == Ticker.Mode.FrameBased, false);
                timeIntervalTab.ButtonSetIsOn((Ticker.Mode)evt.newValue == Ticker.Mode.TimeBased, false);

                realtimeAnimatedContainer.Toggle(evt.newValue.Equals(Ticker.Mode.RealTime));
                frameAnimatedContainer.Toggle(evt.newValue.Equals(Ticker.Mode.FrameBased));
                timeAnimatedContainer.Toggle(evt.newValue.Equals(Ticker.Mode.TimeBased));

                if (!EditorApplication.isPlayingOrWillChangePlaymode) return; //we are not in play mode -> exit
                if (target == null) return;                                   //target is null -> exit
                if (!target.isRunning) return;                                //target is not running -> exit
                target.StartTicking();                                        //target is running -> restart the ticker
            });

            void SetTickerMode(Ticker.Mode tickerMode)
            {
                propertyTickerMode.enumValueIndex = (int)tickerMode;
                property.serializedObject.ApplyModifiedProperties();
            }

            FluidTab GetTickerModeTab(Ticker.Mode tickerMode, string labelText, string tooltipText)
            {
                FluidTab tab =
                    GetNormalTab(labelText, tooltipText)
                        .ButtonSetIsOn((Ticker.Mode)propertyTickerMode.enumValueIndex == tickerMode, false)
                        .ButtonSetOnValueChanged(evt =>
                        {
                            if (!evt.newValue) return;
                            SetTickerMode(tickerMode);
                        });

                switch (tickerMode)
                {
                    case Ticker.Mode.RealTime:
                        tab.SetIcon(EditorSpriteSheets.Bindy.Icons.RealTime);
                        break;
                    case Ticker.Mode.FrameBased:
                        tab.SetIcon(EditorSpriteSheets.Bindy.Icons.FrameBased);
                        break;
                    case Ticker.Mode.TimeBased:
                        tab.SetIcon(EditorSpriteSheets.Bindy.Icons.TimeBased);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(tickerMode), tickerMode, null);
                }

                return tab;
            }

            var tickerModeContainer =
                new VisualElement()
                    .SetName("TickerModeContainer")
                    .SetStyleFlexDirection(FlexDirection.Row)
                    .AddFlexibleSpace()
                    .AddChild(realtimeTab)
                    .AddSpace(1)
                    .AddChild(frameIntervalTab)
                    .AddSpace(1)
                    .AddChild(timeIntervalTab)
                    .AddFlexibleSpace();

            realtimeAnimatedContainer.SetOnShowCallback(() =>
            {
                var maxTicksPerSecondIntField =
                    DesignUtils.NewIntegerField(propertyMaxTicksPerSecond)
                        .SetStyleWidth(60)
                        .SetTooltip
                        (
                            "The maximum number of ticks per second.\n\n" +
                            "-1 means no limit.\n\n" +
                            "If the frame rate is higher than this value, the ticker will not consume more than this number of ticks per second to avoid consuming too many resources."
                        );

                realtimeAnimatedContainer
                    .AddContent
                    (
                        DesignUtils.row
                            .SetStyleAlignItems(Align.Center)
                            .AddFlexibleSpace()
                            .AddChild(DesignUtils.NewFieldNameLabel("Limit to"))
                            .AddSpaceBlock()
                            .AddChild(maxTicksPerSecondIntField)
                            .AddSpaceBlock()
                            .AddChild(DesignUtils.NewFieldNameLabel("Ticks Per Second"))
                            .AddFlexibleSpace()
                    )
                    .Bind(property.serializedObject);
            });

            #endregion

            #region FrameInterval

            frameAnimatedContainer.SetOnShowCallback(() =>
            {
                var framesIntField = DesignUtils.NewIntegerField(propertyFrames).SetStyleWidth(60);

                var frameIntervalsToggleGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.Passive);
                var everyFrameTab = GetFrameTab(Ticker.FrameIntervals.EveryFrame).SetLabelText("1").SetTooltip("Tick every frame");
                var everyTwoFramesTab = GetFrameTab(Ticker.FrameIntervals.EveryTwoFrames).SetLabelText("2").SetTooltip("Tick every two (2) frames");
                var everyThreeFramesTab = GetFrameTab(Ticker.FrameIntervals.EveryThreeFrames).SetLabelText("3").SetTooltip("Tick every three (3) frames");
                var everyFourFramesTab = GetFrameTab(Ticker.FrameIntervals.EveryFourFrames).SetLabelText("4").SetTooltip("Tick every four (4) frames");
                var everyFiveFramesTab = GetFrameTab(Ticker.FrameIntervals.EveryFiveFrames).SetLabelText("5").SetTooltip("Tick every five (5) frames");
                var customFrameIntervalTab = GetFrameTab(Ticker.FrameIntervals.Custom).SetLabelText("Custom").SetTooltip("Set a custom frame interval to tick");

                UpdateFramesIntField((Ticker.FrameIntervals)propertyFrameInterval.enumValueIndex);
                frameIntervalsToggleGroup.SetControlMode(FluidToggleGroup.ControlMode.OneToggleOnEnforced);

                frameIntervalEnumField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    var newValue = (Ticker.FrameIntervals)evt.newValue;
                    switch (newValue)
                    {
                        case Ticker.FrameIntervals.EveryFrame:
                            everyFrameTab.ButtonSetIsOn(true);
                            break;
                        case Ticker.FrameIntervals.EveryTwoFrames:
                            everyTwoFramesTab.ButtonSetIsOn(true);
                            break;
                        case Ticker.FrameIntervals.EveryThreeFrames:
                            everyThreeFramesTab.ButtonSetIsOn(true);
                            break;
                        case Ticker.FrameIntervals.EveryFourFrames:
                            everyFourFramesTab.ButtonSetIsOn(true);
                            break;
                        case Ticker.FrameIntervals.EveryFiveFrames:
                            everyFiveFramesTab.ButtonSetIsOn(true);
                            break;
                        case Ticker.FrameIntervals.Custom:
                            customFrameIntervalTab.ButtonSetIsOn(true);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    UpdateFramesIntField(newValue);
                });

                void SetFrameInterval(Ticker.FrameIntervals frameInterval)
                {
                    propertyFrameInterval.enumValueIndex = (int)frameInterval;
                    propertyFrames.intValue = Ticker.GetFrameInterval(frameInterval, propertyFrames.intValue);
                    property.serializedObject.ApplyModifiedProperties();
                    framesIntField.SetEnabled(frameInterval == Ticker.FrameIntervals.Custom);
                }

                void UpdateFramesIntField(Ticker.FrameIntervals frameInterval)
                {
                    framesIntField.SetEnabled(frameInterval == Ticker.FrameIntervals.Custom);
                    framesIntField.SetTooltip($"Tick every {propertyFrames.intValue} frames");
                }

                FluidTab GetFrameTab(Ticker.FrameIntervals frameInterval) =>
                    GetSmallTab()
                        .ButtonSetIsOn((Ticker.FrameIntervals)propertyFrameInterval.enumValueIndex == frameInterval, false)
                        .AddToToggleGroup(frameIntervalsToggleGroup)
                        .ButtonSetOnValueChanged(evt =>
                        {
                            if (!evt.newValue) return;
                            SetFrameInterval(frameInterval);
                        });

                var frameContainer =
                    new VisualElement()
                        .SetName("FrameIntervalContainer")
                        .AddChild
                        (
                            DesignUtils.row
                                .SetStyleAlignItems(Align.Center)
                                .AddFlexibleSpace()
                                .AddChild(everyFrameTab)
                                .AddSpace(1)
                                .AddChild(everyTwoFramesTab)
                                .AddSpace(1)
                                .AddChild(everyThreeFramesTab)
                                .AddSpace(1)
                                .AddChild(everyFourFramesTab)
                                .AddSpace(1)
                                .AddChild(everyFiveFramesTab)
                                .AddSpace(2)
                                .AddChild(customFrameIntervalTab)
                                .AddFlexibleSpace()
                        )
                        .AddSpaceBlock()
                        .AddChild
                        (
                            DesignUtils.row
                                .SetStyleAlignItems(Align.Center)
                                .AddFlexibleSpace()
                                .AddChild(DesignUtils.NewFieldNameLabel("Tick every"))
                                .AddSpace(DesignUtils.k_Spacing)
                                .AddChild(framesIntField)
                                .AddSpace(DesignUtils.k_Spacing)
                                .AddChild(DesignUtils.NewFieldNameLabel("frames"))
                                .AddFlexibleSpace()
                        );

                frameAnimatedContainer
                    .AddContent(frameContainer)
                    .Bind(property.serializedObject);
            });

            #endregion

            #region TimeInterval

            timeAnimatedContainer.SetOnShowCallback(() =>
            {
                var secondsFloatField = DesignUtils.NewFloatField(propertySeconds).SetStyleWidth(60);

                var timeIntervalsToggleGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.Passive);
                var oncePerSecondTab = GetTimeTab(Ticker.TimeIntervals.OncePerSecond).SetLabelText("1").SetTooltip("Tick once per second");
                var twicePerSecondTab = GetTimeTab(Ticker.TimeIntervals.TwicePerSecond).SetLabelText("2").SetTooltip("Tick twice per second (0.5 seconds)");
                var fiveTimesPerSecondTab = GetTimeTab(Ticker.TimeIntervals.FiveTimesPerSecond).SetLabelText("5").SetTooltip("Tick five times per second (0.2 seconds)");
                var tenTimesPerSecondTab = GetTimeTab(Ticker.TimeIntervals.TenTimesPerSecond).SetLabelText("10").SetTooltip("Tick ten times per second (0.1 seconds)");
                var twentyTimesPerSecondTab = GetTimeTab(Ticker.TimeIntervals.TwentyTimesPerSecond).SetLabelText("20").SetTooltip("Tick twenty times per second (0.05 seconds)");
                var thirtyTimesPerSecondTab = GetTimeTab(Ticker.TimeIntervals.ThirtyTimesPerSecond).SetLabelText("30").SetTooltip("Tick thirty times per second (0.033 seconds)");
                var sixtyTimesPerSecondTab = GetTimeTab(Ticker.TimeIntervals.SixtyTimesPerSecond).SetLabelText("60").SetTooltip("Tick sixty times per second (0.016 seconds)");
                var customTimeIntervalTab = GetTimeTab(Ticker.TimeIntervals.Custom).SetLabelText("Custom").SetTooltip("Set a custom time interval to tick");

                UpdateSecondsFloatField((Ticker.TimeIntervals)propertyTimeInterval.enumValueIndex);
                timeIntervalsToggleGroup.SetControlMode(FluidToggleGroup.ControlMode.OneToggleOnEnforced);

                timeIntervalEnumField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    var newValue = (Ticker.TimeIntervals)evt.newValue;
                    switch (newValue)
                    {
                        case Ticker.TimeIntervals.OncePerSecond:
                            oncePerSecondTab.ButtonSetIsOn(true, false);
                            break;
                        case Ticker.TimeIntervals.TwicePerSecond:
                            twicePerSecondTab.ButtonSetIsOn(true, false);
                            break;
                        case Ticker.TimeIntervals.FiveTimesPerSecond:
                            fiveTimesPerSecondTab.ButtonSetIsOn(true, false);
                            break;
                        case Ticker.TimeIntervals.TenTimesPerSecond:
                            tenTimesPerSecondTab.ButtonSetIsOn(true, false);
                            break;
                        case Ticker.TimeIntervals.TwentyTimesPerSecond:
                            twentyTimesPerSecondTab.ButtonSetIsOn(true, false);
                            break;
                        case Ticker.TimeIntervals.ThirtyTimesPerSecond:
                            thirtyTimesPerSecondTab.ButtonSetIsOn(true, false);
                            break;
                        case Ticker.TimeIntervals.SixtyTimesPerSecond:
                            sixtyTimesPerSecondTab.ButtonSetIsOn(true, false);
                            break;
                        case Ticker.TimeIntervals.Custom:
                            customTimeIntervalTab.ButtonSetIsOn(true, false);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    UpdateSecondsFloatField(newValue);
                });

                void SetTimeInterval(Ticker.TimeIntervals timeInterval)
                {
                    propertyTimeInterval.enumValueIndex = (int)timeInterval;
                    propertySeconds.floatValue = Ticker.GetTimeInterval(timeInterval, propertySeconds.floatValue);
                    property.serializedObject.ApplyModifiedProperties();
                    secondsFloatField.SetEnabled(timeInterval == Ticker.TimeIntervals.Custom);
                }

                void UpdateSecondsFloatField(Ticker.TimeIntervals timeInterval)
                {
                    secondsFloatField.SetEnabled(timeInterval == Ticker.TimeIntervals.Custom);
                    secondsFloatField.SetTooltip($"Tick every {propertySeconds.floatValue} seconds");
                }

                FluidTab GetTimeTab(Ticker.TimeIntervals timeInterval) =>
                    GetSmallTab()
                        .ButtonSetIsOn((Ticker.TimeIntervals)propertyTimeInterval.enumValueIndex == timeInterval, false)
                        .AddToToggleGroup(timeIntervalsToggleGroup)
                        .ButtonSetOnValueChanged(evt =>
                        {
                            if (!evt.newValue) return;
                            SetTimeInterval(timeInterval);
                        });

                var timeContainer =
                    new VisualElement()
                        .SetName("TimeIntervalContainer")
                        .AddChild
                        (
                            DesignUtils.row
                                .SetStyleAlignItems(Align.Center)
                                .AddFlexibleSpace()
                                .AddChild(sixtyTimesPerSecondTab)
                                .AddSpace(1)
                                .AddChild(thirtyTimesPerSecondTab)
                                .AddSpace(1)
                                .AddChild(twentyTimesPerSecondTab)
                                .AddSpace(1)
                                .AddChild(tenTimesPerSecondTab)
                                .AddSpace(1)
                                .AddChild(fiveTimesPerSecondTab)
                                .AddSpace(1)
                                .AddChild(twicePerSecondTab)
                                .AddSpace(1)
                                .AddChild(oncePerSecondTab)
                                .AddSpace(2)
                                .AddChild(customTimeIntervalTab)
                                .AddFlexibleSpace()
                        )
                        .AddSpaceBlock()
                        .AddChild
                        (
                            DesignUtils.row
                                .SetStyleAlignItems(Align.Center)
                                .AddFlexibleSpace()
                                .AddChild(DesignUtils.NewFieldNameLabel("Tick every"))
                                .AddSpace(DesignUtils.k_Spacing)
                                .AddChild(secondsFloatField)
                                .AddSpace(DesignUtils.k_Spacing)
                                .AddChild(DesignUtils.NewFieldNameLabel("seconds"))
                                .AddFlexibleSpace()
                        );

                timeAnimatedContainer
                    .AddContent(timeContainer)
                    .Bind(property.serializedObject);
            });

            #endregion

            return
                drawer
                    .AddSpaceBlock()
                    .AddChild(tickerModeEnumField)
                    .AddChild(frameIntervalEnumField)
                    .AddChild(timeIntervalEnumField)
                    .AddChild(tickerModeContainer)
                    .AddSpaceBlock()
                    .AddChild(realtimeAnimatedContainer)
                    .AddChild(frameAnimatedContainer)
                    .AddChild(timeAnimatedContainer)
                    .AddSpaceBlock()
                ;

            FluidTab GetNormalTab(string labelText, string tooltipText)
            {
                var tab =
                    FluidTab.Get()
                        .SetLabelText(labelText)
                        .SetTooltip(tooltipText)
                        .SetElementSize(ElementSize.Normal)
                        .ButtonSetAccentColor(tabSelectableColor)
                        .AddToToggleGroup(tickerModeToggleGroup);

                tab.indicator.SetStyleDisplay(DisplayStyle.None);

                return tab;
            }

            FluidTab GetSmallTab()
            {
                FluidTab tab =
                    FluidTab.Get()
                        .SetElementSize(ElementSize.Small)
                        .SetTabPosition(TabPosition.FloatingTab)
                        .ButtonSetAccentColor(tabSelectableColor);

                tab.indicator.SetStyleDisplay(DisplayStyle.None);

                return tab;
            }
        }
    }
}
