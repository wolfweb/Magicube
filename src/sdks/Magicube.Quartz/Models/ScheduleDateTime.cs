using System;
using System.Text.RegularExpressions;

namespace Magicube.Quartz.Models {
    [Serializable]
    public class ScheduleDateTime {
        public ScheduleDateTime(int hour, int minute, int second) {
            Hour = hour;
            Minute = minute;
            Second = second;
            Validate();
        }
        public int Hour             { get; set; }
        public int Minute           { get; set; }
        public int Second           { get; set; }
                                    
        public int Day              { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }

        public string ToString(string format) {
            var result = Regex.Replace(format, "d+", Day    < 10 ? $"0{Day}"    : Day.ToString());
                result = Regex.Replace(result, "h+", Hour   < 10 ? $"0{Hour}"   : Hour.ToString());
                result = Regex.Replace(result, "s+", Second < 10 ? $"0{Second}" : Second.ToString());
                result = Regex.Replace(result, "m+", Minute < 10 ? $"0{Minute}" : Minute.ToString());
            return result;
        }

        private void Validate() {
            if (Hour < 0 || Hour > 23)
                throw new ArgumentException("Hour must be from 0 to 23");
            if (Minute < 0 || Minute > 59)
                throw new ArgumentException("Minute must be from 0 to 59");
            if (Second < 0 || Second > 59)
                throw new ArgumentException("Second must be from 0 to 59");
        }
    }
}
