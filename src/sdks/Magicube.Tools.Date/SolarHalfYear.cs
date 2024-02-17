using System;
using System.Collections.Generic;

namespace Magicube.Tools.Date {
    public class SolarHalfYear {
        public int Year  { get; }

        public int Month { get; }

        public const int MONTH_COUNT = 6;

        public SolarHalfYear() : this(DateTime.Now) {
        }

        public SolarHalfYear(DateTime date) {
            Year  = date.Year;
            Month = date.Month;
        }

        public SolarHalfYear(int year, int month) {
            this.Year = year;
            this.Month = month;
        }

        public int GetIndex() {
            return (int)Math.Ceiling(Month * 1D / MONTH_COUNT);
        }

        public SolarHalfYear Next(int halfYears) {
            if (0 == halfYears) {
                return new SolarHalfYear(Year, Month);
            }
            DateTime c = new DateTime(Year, Month, 1);
            c = c.AddMonths(MONTH_COUNT * halfYears);
            return new SolarHalfYear(c);
        }

        public List<SolarMonth> GetMonths() {
            List<SolarMonth> l = new List<SolarMonth>();
            int index = GetIndex() - 1;
            for (int i = 0; i < MONTH_COUNT; i++) {
                l.Add(new SolarMonth(Year, MONTH_COUNT * index + i + 1));
            }
            return l;
        }

        public override string ToString() {
            return Year + "." + GetIndex();
        }

        public string ToFullString() {
            return Year + "年" + (GetIndex() == 1 ? "上" : "下") + "半年";
        }
        public static SolarHalfYear FromDate(DateTime date) {
            return new SolarHalfYear(date);
        }

        public static SolarHalfYear FromYm(int year, int month) {
            return new SolarHalfYear(year, month);
        }
    }
}
