using System;
using System.Collections.Generic;

namespace Magicube.Tools.Date {
    public class SolarYear {
        public int Year { get; }

        public const int MONTH_COUNT = 12;

        public SolarYear()
            : this(DateTime.Now) {
        }

        public SolarYear(DateTime date) {
            Year = date.Year;
        }

        public SolarYear(int year) {
            Year = year;
        }

        public List<SolarMonth> GetMonths() {
            List<SolarMonth> l = new List<SolarMonth>(MONTH_COUNT);
            SolarMonth m = new SolarMonth(Year, 1);
            l.Add(m);
            for (int i = 1; i < MONTH_COUNT; i++) {
                l.Add(m.Next(i));
            }
            return l;
        }

        public SolarYear Next(int years) {
            DateTime c = new DateTime(Year, 1, 1);
            c = c.AddYears(years);
            return new SolarYear(c);
        }

        public override string ToString() {
            return Year + "";
        }

        public string ToFullString() {
            return Year + "年";
        }
        public static SolarYear FromDate(DateTime date) {
            return new SolarYear(date);
        }

        public static SolarYear FromYear(int year) {
            return new SolarYear(year);
        }
    }
}
