using System;
using System.Collections.Generic;

namespace Magicube.Tools.Date {
    public class SolarSeason {
        public int Year  { get; }
        public int Month { get; }

        public const int MONTH_COUNT = 3;

        public SolarSeason(): this(DateTime.Now) {
        }

        public SolarSeason(DateTime date) {
            Year  = date.Year;
            Month = date.Month;
        }

        public SolarSeason(int year, int month) {
            Year = year;
            Month = month;
        }

        public int GetIndex() {
            return (int)Math.Ceiling(Month * 1D / MONTH_COUNT);
        }

        public SolarSeason Next(int seasons) {
            if (0 == seasons) {
                return new SolarSeason(Year, Month);
            }
            DateTime c = new DateTime(Year, Month, 1);
            c = c.AddMonths(MONTH_COUNT * seasons);
            return new SolarSeason(c);
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
            return Year + "年" + GetIndex() + "季度";
        }
        public static SolarSeason FromDate(DateTime date) {
            return new SolarSeason(date);
        }

        public static SolarSeason FromYm(int year, int month) {
            return new SolarSeason(year, month);
        }
    }
}
