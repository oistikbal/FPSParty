// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Bindy.Transformers;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine.UIElements;

namespace Doozy.Editor.Bindy.Editors.Transformers
{
    [CustomEditor(typeof(DateTimeToTimeAgoTransformer), true)]
    public class DateTimeToTimeAgoTransformerEditor : ValueTransformerEditor
    {
        protected override bool customEditor => true;

        private SerializedProperty propertyJustNowText { get; set; }
        private SerializedProperty propertyPastTextFormat { get; set; }
        private SerializedProperty propertyFutureTextFormat { get; set; }
        private SerializedProperty propertySecondTextSingular { get; set; }
        private SerializedProperty propertySecondTextPlural { get; set; }
        private SerializedProperty propertyMinuteTextSingular { get; set; }
        private SerializedProperty propertyMinuteTextPlural { get; set; }
        private SerializedProperty propertyHourTextSingular { get; set; }
        private SerializedProperty propertyHourTextPlural { get; set; }
        private SerializedProperty propertyDayTextSingular { get; set; }
        private SerializedProperty propertyDayTextPlural { get; set; }
        private SerializedProperty propertyWeekTextSingular { get; set; }
        private SerializedProperty propertyWeekTextPlural { get; set; }
        private SerializedProperty propertyMonthTextSingular { get; set; }
        private SerializedProperty propertyMonthTextPlural { get; set; }
        private SerializedProperty propertyYearTextSingular { get; set; }
        private SerializedProperty propertyYearTextPlural { get; set; }
        private SerializedProperty propertyYesterdayText { get; set; }
        private SerializedProperty propertyTomorrowText { get; set; }
        private SerializedProperty propertyLastWeekText { get; set; }
        private SerializedProperty propertyNextWeekText { get; set; }
        private SerializedProperty propertyLastMonthText { get; set; }
        private SerializedProperty propertyNextMonthText { get; set; }
        private SerializedProperty propertyLastYearText { get; set; }
        private SerializedProperty propertyNextYearText { get; set; }


        protected override void FindSerializedProperties()
        {
            base.FindSerializedProperties();
            propertyJustNowText = serializedObject.FindProperty("JustNowText");
            propertyPastTextFormat = serializedObject.FindProperty("PastTextFormat");
            propertyFutureTextFormat = serializedObject.FindProperty("FutureTextFormat");
            propertySecondTextSingular = serializedObject.FindProperty("SecondTextSingular");
            propertySecondTextPlural = serializedObject.FindProperty("SecondTextPlural");
            propertyMinuteTextSingular = serializedObject.FindProperty("MinuteTextSingular");
            propertyMinuteTextPlural = serializedObject.FindProperty("MinuteTextPlural");
            propertyHourTextSingular = serializedObject.FindProperty("HourTextSingular");
            propertyHourTextPlural = serializedObject.FindProperty("HourTextPlural");
            propertyDayTextSingular = serializedObject.FindProperty("DayTextSingular");
            propertyDayTextPlural = serializedObject.FindProperty("DayTextPlural");
            propertyWeekTextSingular = serializedObject.FindProperty("WeekTextSingular");
            propertyWeekTextPlural = serializedObject.FindProperty("WeekTextPlural");
            propertyMonthTextSingular = serializedObject.FindProperty("MonthTextSingular");
            propertyMonthTextPlural = serializedObject.FindProperty("MonthTextPlural");
            propertyYearTextSingular = serializedObject.FindProperty("YearTextSingular");
            propertyYearTextPlural = serializedObject.FindProperty("YearTextPlural");
            propertyYesterdayText = serializedObject.FindProperty("YesterdayText");
            propertyTomorrowText = serializedObject.FindProperty("TomorrowText");
            propertyLastWeekText = serializedObject.FindProperty("LastWeekText");
            propertyNextWeekText = serializedObject.FindProperty("NextWeekText");
            propertyLastMonthText = serializedObject.FindProperty("LastMonthText");
            propertyNextMonthText = serializedObject.FindProperty("NextMonthText");
            propertyLastYearText = serializedObject.FindProperty("LastYearText");
            propertyNextYearText = serializedObject.FindProperty("NextYearText");
        }

        protected override void InitializeCustomInspector()
        {
            TextField justNowTextField =
                DesignUtils.NewTextField(propertyJustNowText)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to display when the time difference is less than 1 second");

            FluidField justNowFluidField =
                FluidField.Get()
                    .SetLabelText("Just Now")
                    .AddFieldContent(justNowTextField);

            TextField pastTextFormatField =
                DesignUtils.NewTextField(propertyPastTextFormat)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text format to use when the time difference is in the past");

            FluidField pastTextFormatFluidField =
                FluidField.Get()
                    .SetLabelText("Past Text Format")
                    .AddFieldContent(pastTextFormatField);

            TextField futureTextFormatField =
                DesignUtils.NewTextField(propertyFutureTextFormat)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text format to use when the time difference is in the future");

            FluidField futureTextFormatFluidField =
                FluidField.Get()
                    .SetLabelText("Future Text Format")
                    .AddFieldContent(futureTextFormatField);

            TextField secondTextSingularField =
                DesignUtils.NewTextField(propertySecondTextSingular)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is 1 second");

            FluidField secondTextSingularFluidField =
                FluidField.Get()
                    .SetLabelText("Second")
                    .AddFieldContent(secondTextSingularField);

            TextField secondTextPluralField =
                DesignUtils.NewTextField(propertySecondTextPlural)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is more than 1 second");

            FluidField secondTextPluralFluidField =
                FluidField.Get()
                    .SetLabelText("Seconds")
                    .AddFieldContent(secondTextPluralField);

            TextField minuteTextSingularField =
                DesignUtils.NewTextField(propertyMinuteTextSingular)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is 1 minute");

            FluidField minuteTextSingularFluidField =
                FluidField.Get()
                    .SetLabelText("Minute")
                    .AddFieldContent(minuteTextSingularField);

            TextField minuteTextPluralField =
                DesignUtils.NewTextField(propertyMinuteTextPlural)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is more than 1 minute");

            FluidField minuteTextPluralFluidField =
                FluidField.Get()
                    .SetLabelText("Minutes")
                    .AddFieldContent(minuteTextPluralField);

            TextField hourTextSingularField =
                DesignUtils.NewTextField(propertyHourTextSingular)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is 1 hour");

            FluidField hourTextSingularFluidField =
                FluidField.Get()
                    .SetLabelText("Hour")
                    .AddFieldContent(hourTextSingularField);

            TextField hourTextPluralField =
                DesignUtils.NewTextField(propertyHourTextPlural)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is more than 1 hour");

            FluidField hourTextPluralFluidField =
                FluidField.Get()
                    .SetLabelText("Hours")
                    .AddFieldContent(hourTextPluralField);

            TextField dayTextSingularField =
                DesignUtils.NewTextField(propertyDayTextSingular)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is 1 day");

            FluidField dayTextSingularFluidField =
                FluidField.Get()
                    .SetLabelText("Day")
                    .AddFieldContent(dayTextSingularField);

            TextField dayTextPluralField =
                DesignUtils.NewTextField(propertyDayTextPlural)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is more than 1 day");

            FluidField dayTextPluralFluidField =
                FluidField.Get()
                    .SetLabelText("Days")
                    .AddFieldContent(dayTextPluralField);

            TextField weekTextSingularField =
                DesignUtils.NewTextField(propertyWeekTextSingular)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is 1 week");

            FluidField weekTextSingularFluidField =
                FluidField.Get()
                    .SetLabelText("Week")
                    .AddFieldContent(weekTextSingularField);

            TextField weekTextPluralField =
                DesignUtils.NewTextField(propertyWeekTextPlural)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is more than 1 week");

            FluidField weekTextPluralFluidField =
                FluidField.Get()
                    .SetLabelText("Weeks")
                    .AddFieldContent(weekTextPluralField);

            TextField monthTextSingularField =
                DesignUtils.NewTextField(propertyMonthTextSingular)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is 1 month");

            FluidField monthTextSingularFluidField =
                FluidField.Get()
                    .SetLabelText("Month")
                    .AddFieldContent(monthTextSingularField);

            TextField monthTextPluralField =
                DesignUtils.NewTextField(propertyMonthTextPlural)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is more than 1 month");

            FluidField monthTextPluralFluidField =
                FluidField.Get()
                    .SetLabelText("Months")
                    .AddFieldContent(monthTextPluralField);

            TextField yearTextSingularField =
                DesignUtils.NewTextField(propertyYearTextSingular)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is 1 year");

            FluidField yearTextSingularFluidField =
                FluidField.Get()
                    .SetLabelText("Year")
                    .AddFieldContent(yearTextSingularField);

            TextField yearTextPluralField =
                DesignUtils.NewTextField(propertyYearTextPlural)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is more than 1 year");

            FluidField yearTextPluralFluidField =
                FluidField.Get()
                    .SetLabelText("Years")
                    .AddFieldContent(yearTextPluralField);

            TextField yesterdayTextField =
                DesignUtils.NewTextField(propertyYesterdayText)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is 1 day");

            FluidField yesterdayTextFluidField =
                FluidField.Get()
                    .SetLabelText("Yesterday")
                    .AddFieldContent(yesterdayTextField);

            TextField tomorrowTextField =
                DesignUtils.NewTextField(propertyTomorrowText)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is 1 day");

            FluidField tomorrowTextFluidField =
                FluidField.Get()
                    .SetLabelText("Tomorrow")
                    .AddFieldContent(tomorrowTextField);

            TextField lastWeekTextField =
                DesignUtils.NewTextField(propertyLastWeekText)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is 1 week");

            FluidField lastWeekTextFluidField =
                FluidField.Get()
                    .SetLabelText("Last Week")
                    .AddFieldContent(lastWeekTextField);

            TextField nextWeekTextField =
                DesignUtils.NewTextField(propertyNextWeekText)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is 1 week");

            FluidField nextWeekTextFluidField =
                FluidField.Get()
                    .SetLabelText("Next Week")
                    .AddFieldContent(nextWeekTextField);

            TextField lastMonthTextField =
                DesignUtils.NewTextField(propertyLastMonthText)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is 1 month");

            FluidField lastMonthTextFluidField =
                FluidField.Get()
                    .SetLabelText("Last Month")
                    .AddFieldContent(lastMonthTextField);

            TextField nextMonthTextField =
                DesignUtils.NewTextField(propertyNextMonthText)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is 1 month");

            FluidField nextMonthTextFluidField =
                FluidField.Get()
                    .SetLabelText("Next Month")
                    .AddFieldContent(nextMonthTextField);

            TextField lastYearTextField =
                DesignUtils.NewTextField(propertyLastYearText)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is 1 year");

            FluidField lastYearTextFluidField =
                FluidField.Get()
                    .SetLabelText("Last Year")
                    .AddFieldContent(lastYearTextField);

            TextField nextYearTextField =
                DesignUtils.NewTextField(propertyNextYearText)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("The text to use when the time difference is 1 year");

            FluidField nextYearTextFluidField =
                FluidField.Get()
                    .SetLabelText("Next Year")
                    .AddFieldContent(nextYearTextField);

            contentContainer
                .AddChild(justNowFluidField)
                .AddSpaceBlock()
                .AddChild(pastTextFormatFluidField)
                .AddSpaceBlock()
                .AddChild(futureTextFormatFluidField)
                .AddSpaceBlock(2)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(secondTextSingularFluidField)
                        .AddSpaceBlock()
                        .AddChild(secondTextPluralFluidField)
                )
                .AddSpaceBlock()
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(minuteTextSingularFluidField)
                        .AddSpaceBlock()
                        .AddChild(minuteTextPluralFluidField)
                )
                .AddSpaceBlock()
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(hourTextSingularFluidField)
                        .AddSpaceBlock()
                        .AddChild(hourTextPluralFluidField)
                )
                .AddSpaceBlock()
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(dayTextSingularFluidField)
                        .AddSpaceBlock()
                        .AddChild(dayTextPluralFluidField)
                )
                .AddSpaceBlock()
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(weekTextSingularFluidField)
                        .AddSpaceBlock()
                        .AddChild(weekTextPluralFluidField)
                )
                .AddSpaceBlock()
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(monthTextSingularFluidField)
                        .AddSpaceBlock()
                        .AddChild(monthTextPluralFluidField)
                )
                .AddSpaceBlock()
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(yearTextSingularFluidField)
                        .AddSpaceBlock()
                        .AddChild(yearTextPluralFluidField)
                )
                .AddSpaceBlock(2)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(yesterdayTextFluidField)
                        .AddSpaceBlock()
                        .AddChild(tomorrowTextFluidField)
                )
                .AddSpaceBlock()
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(lastWeekTextFluidField)
                        .AddSpaceBlock()
                        .AddChild(nextWeekTextFluidField)
                )
                .AddSpaceBlock()
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(lastMonthTextFluidField)
                        .AddSpaceBlock()
                        .AddChild(nextMonthTextFluidField)
                )
                .AddSpaceBlock()
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(lastYearTextFluidField)
                        .AddSpaceBlock()
                        .AddChild(nextYearTextFluidField)
                );

        }
    }
}
