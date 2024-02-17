using System;

namespace Magicube.Tools.Date {
    public class Yun {        
        public int   Gender     { get; }
        public int   StartYear  { get; private set; }
        public int   StartMonth { get; private set; }
        public int   StartDay   { get; private set; }
        public bool  Forward    { get; }
        public Lunar Lunar      { get; }

        public Yun(EightChar eightChar, int gender) {
            this.Lunar = eightChar.getLunar();
            this.Gender = gender;
            bool yang = 0 == Lunar.YearGanIndexExact % 2;
            bool man = 1 == gender;
            Forward = (yang && man) || (!yang && !man);
            ComputeStart();
        }

        private void ComputeStart() {
            JieQi prev = Lunar.getPrevJie();
            JieQi next = Lunar.getNextJie();
            Solar current = Lunar.Solar;
            Solar start = Forward ? current : prev.Solar;
            Solar end = Forward ? next.Solar : current;
            int hourDiff = LunarUtil.getTimeZhiIndex(end.toYmdHms().Substring(11, 5)) - LunarUtil.getTimeZhiIndex(start.toYmdHms().Substring(11, 5));
            DateTime endCalendar = new DateTime(end.getYear(), end.getMonth(), end.getDay(), 0, 0, 0, 0);
            DateTime startCalendar = new DateTime(start.getYear(), start.getMonth(), start.getDay(), 0, 0, 0, 0);
            int dayDiff = endCalendar.Subtract(startCalendar).Days;
            if (hourDiff < 0) {
                hourDiff += 12;
                dayDiff--;
            }
            int monthDiff = hourDiff * 10 / 30;
            int month = dayDiff * 4 + monthDiff;
            int day = hourDiff * 10 - monthDiff * 30;
            int year = month / 12;
            month = month - year * 12;
            StartYear = year;
            StartMonth = month;
            StartDay = day;
        }

        public Solar GetStartSolar() {
            Solar birth = Lunar.Solar;
            DateTime c = new DateTime(birth.getYear(), birth.getMonth(), birth.getDay(), 0, 0, 0);
            c = c.AddYears(StartYear);
            c = c.AddMonths(StartMonth);
            c = c.AddDays(StartDay);
            return Solar.fromDate(c);
        }

        public DaYun[] GetDaYun() {
            int n = 10;
            DaYun[] l = new DaYun[n];
            for (int i = 0; i < n; i++) {
                l[i] = new DaYun(this, i);
            }
            return l;
        }

    }
}
