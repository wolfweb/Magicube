using System;
using System.Collections.Generic;

namespace Magicube.Tools.Date {
    public class SolarMonth {
        public int Year  { get; }

        public int Month { get; }
        public SolarMonth(): this(DateTime.Now) {
        }

        public SolarMonth(DateTime date) {
            Year  = date.Year;
            Month = date.Month;
        }
        public SolarMonth(int year, int month) {
            this.Year = year;
            this.Month = month;
        }

        public List<Solar> GetDays() {
            List<Solar> l = new List<Solar>(31);
            Solar d = new Solar(Year, Month, 1);
            l.Add(d);
            int days = SolarUtil.GetDaysOfMonth(Year, Month);
            for (int i = 1; i < days; i++) {
                l.Add(d.next(i));
            }
            return l;
        }

        public SolarMonth Next(int months) {
            DateTime c = new DateTime(Year, Month, 1);
            c = c.AddMonths(months);
            return new SolarMonth(c);
        }

        public override string ToString() {
            return Year + "-" + Month;
        }

        public string ToFullString() {
            return Year + "年" + Month + "月";
        }
        public static SolarMonth FromDate(DateTime date) {
            return new SolarMonth(date);
        }

        public static SolarMonth FromYm(int year, int month) {
            return new SolarMonth(year, month);
        }
    }
}
