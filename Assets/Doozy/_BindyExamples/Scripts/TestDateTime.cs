using System;
using UnityEngine;
// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Sandbox.Bindy
{
    public class TestDateTime : MonoBehaviour
    {
        public DateTime Time;

        private void OnEnable()
        {
            SetCurrentDate();
        }

        public void SetYesterday() => Time = DateTime.UtcNow.AddDays(-1);
        public void SetCurrentDate() => Time = DateTime.UtcNow;
        public void SetTomorrow() => Time = DateTime.UtcNow.AddDays(1).AddSeconds(1);

        public void AddSecond() => Time = Time.AddSeconds(1);
        public void AddMinute() => Time = Time.AddMinutes(1);
        public void AddHour() => Time = Time.AddHours(1);
        public void AddDay() => Time = Time.AddDays(1);
        public void AddWeek() => Time = Time.AddDays(7);
        public void AddMonth() => Time = Time.AddMonths(1);
        public void AddYear() => Time = Time.AddYears(1);

        public void SubtractSecond() => Time = Time.AddSeconds(-1);
        public void SubtractMinute() => Time = Time.AddMinutes(-1);
        public void SubtractHour() => Time = Time.AddHours(-1);
        public void SubtractDay() => Time = Time.AddDays(-1);
        public void SubtractWeek() => Time = Time.AddDays(-7);
        public void SubtractMonth() => Time = Time.AddMonths(-1);
        public void SubtractYear() => Time = Time.AddYears(-1);

        public void ChangeYear(int value) => Time = Time.AddYears(value);
        public void ChangeMonth(int value) => Time = Time.AddMonths(value);
        public void ChangeDay(int value) => Time = Time.AddDays(value);
        public void ChangeHour(int value) => Time = Time.AddHours(value);
        public void ChangeMinute(int value) => Time = Time.AddMinutes(value);
        public void ChangeSecond(int value) => Time = Time.AddSeconds(value);
    }
}
