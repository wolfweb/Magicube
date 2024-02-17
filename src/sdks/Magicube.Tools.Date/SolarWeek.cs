using System;
using System.Collections.Generic;

namespace Magicube.Tools.Date {
    public class SolarWeek {
        public int Year  { get; }

        public int Month { get; }

        public int Day   { get; }

        public int Start { get; }

        public SolarWeek(int start) : this(DateTime.Now, start) {
        }

        public SolarWeek(DateTime date, int start) : this(date.Year, date.Month, date.Day, start) {
        }

        public SolarWeek(int year, int month, int day, int start) {
            Year  = year;
            Month = month;
            Day   = day;
            Start = start;
        }

        public int GetIndex() {
            DateTime firstDay = new DateTime(Year, Month, 1);
            int firstDayWeek = Convert.ToInt32(firstDay.DayOfWeek.ToString("d"));
            if (firstDayWeek == 0) {
                firstDayWeek = 7;
            }
            return (int)Math.Ceiling((Day + firstDayWeek - Start) * 1D / SolarUtil.WEEK.Length);
        }

        public SolarWeek Next(int weeks, bool separateMonth) {
            if (0 == weeks) {
                return new SolarWeek(Year, Month, Day, Start);
            }
            if (separateMonth) {
                int n = weeks;
                DateTime c = new DateTime(Year, Month, Day);
                SolarWeek week = new SolarWeek(c, Start);
                int m = this.Month;
                bool plus = n > 0;
                while (0 != n) {
                    c = c.AddDays(plus ? 7 : -7);
                    week = new SolarWeek(c, Start);
                    int weekMonth = week.Month;
                    if (m != weekMonth) {
                        int index = week.GetIndex();
                        if (plus) {
                            if (1 == index) {
                                Solar firstDay = week.GetFirstDay();
                                week = new SolarWeek(firstDay.getYear(), firstDay.getMonth(), firstDay.getDay(), Start);
                                weekMonth = week.Month;
                            } else {
                                c = new DateTime(week.Year, week.Month, 1);
                                week = new SolarWeek(c, Start);
                            }
                        } else {
                            int size = SolarUtil.GetWeeksOfMonth(week.Year, week.Month, Start);
                            if (size == index) {
                                Solar firstDay = week.GetFirstDay();
                                Solar lastDay = firstDay.next(6);
                                week = new SolarWeek(lastDay.getYear(), lastDay.getMonth(), lastDay.getDay(), Start);
                                weekMonth = week.Month;
                            } else {
                                c = new DateTime(week.Year, week.Month, SolarUtil.GetDaysOfMonth(week.Year, week.Month));
                                week = new SolarWeek(c, Start);
                            }
                        }
                        m = weekMonth;
                    }
                    n -= plus ? 1 : -1;
                }
                return week;
            } else {
                DateTime c = new DateTime(Year, Month, Day);
                c = c.AddDays(weeks * 7);
                return new SolarWeek(c, Start);
            }
        }

        public Solar GetFirstDay() {
            DateTime c = new DateTime(Year, Month, Day);
            int week = Convert.ToInt32(c.DayOfWeek.ToString("d"));
            int prev = week - Start;
            if (prev < 0) {
                prev += 7;
            }
            c = c.AddDays(-prev);
            return new Solar(c);
        }


        public Solar GetFirstDayInMonth() {
            List<Solar> days = GetDays();
            foreach (Solar day in days) {
                if (Month == day.getMonth()) {
                    return day;
                }
            }
            return null;
        }


        public List<Solar> GetDays() {
            Solar firstDay = GetFirstDay();
            List<Solar> l = new List<Solar>();
            l.Add(firstDay);
            for (int i = 1; i < 7; i++) {
                l.Add(firstDay.next(i));
            }
            return l;
        }


        public List<Solar> GetDaysInMonth() {
            List<Solar> days = this.GetDays();
            List<Solar> l = new List<Solar>();
            foreach (Solar day in days) {
                if (Month != day.getMonth()) {
                    continue;
                }
                l.Add(day);
            }
            return l;
        }

        public override string ToString() {
            return Year + "." + Month + "." + GetIndex();
        }

        public string ToFullString() {
            return Year + "年" + Month + "月第" + GetIndex() + "周";
        }
        public static SolarWeek FromDate(DateTime date, int start) {
            return new SolarWeek(date, start);
        }

        public static SolarWeek FromYmd(int year, int month, int day, int start) {
            return new SolarWeek(year, month, day, start);
        }
    }
}
