using System;
using System.Collections.Generic;
using System.Text;

namespace Magicube.Tools.Date {
    public class Solar {

        public const double J2000 = 2451545;

        private int year;

        private int month;

        private int day;

        private int hour;

        private int minute;

        private int second;

        private DateTime calendar;

        public Solar()
            : this(DateTime.Now) {
        }

        public Solar(int year, int month, int day)
            : this(year, month, day, 0, 0, 0) {
        }

        public Solar(int year, int month, int day, int hour, int minute, int second) {
            this.year = year;
            this.month = month;
            this.day = day;
            this.hour = hour;
            this.minute = minute;
            this.second = second;
            this.calendar = new DateTime(year, month, day, hour, minute, second);
        }

        public Solar(DateTime date)
            : this(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second) {
        }

        public Solar(double julianDay) {
            int d = (int)(julianDay + 0.5);
            double f = julianDay + 0.5 - d;
            int c;

            if (d >= 2299161) {
                c = (int)((d - 1867216.25) / 36524.25);
                d += 1 + c - (int)(c * 1D / 4);
            }
            d += 1524;
            int year = (int)((d - 122.1) / 365.25);
            d -= (int)(365.25 * year);
            int month = (int)(d * 1D / 30.601);
            d -= (int)(30.601 * month);
            int day = d;
            if (month > 13) {
                month -= 13;
                year -= 4715;
            } else {
                month -= 1;
                year -= 4716;
            }
            f *= 24;
            int hour = (int)f;

            f -= hour;
            f *= 60;
            int minute = (int)f;

            f -= minute;
            f *= 60;
            int second = (int)Math.Round(f);

            if (second == 60) {
                second = 59;
            }

            calendar = new DateTime(year, month, day, hour, minute, second);
            this.year = year;
            this.month = month;
            this.day = day;
            this.hour = hour;
            this.minute = minute;
            this.second = second;
        }

        public static List<Solar> fromBaZi(string yearGanZhi, string monthGanZhi, string dayGanZhi, string timeGanZhi) {
            return fromBaZi(yearGanZhi, monthGanZhi, dayGanZhi, timeGanZhi, 2);
        }

        public static List<Solar> fromBaZi(string yearGanZhi, string monthGanZhi, string dayGanZhi, string timeGanZhi, int sect) {
            sect = (1 == sect) ? 1 : 2;
            List<Solar> l = new List<Solar>();
            Solar today = new Solar();
            Lunar lunar = today.GetLunar();
            int offsetYear = LunarUtil.getJiaZiIndex(lunar.GetYearInGanZhiExact()) - LunarUtil.getJiaZiIndex(yearGanZhi);
            if (offsetYear < 0) {
                offsetYear = offsetYear + 60;
            }
            int startYear = today.getYear() - offsetYear;
            int hour = 0;
            string timeZhi = timeGanZhi.Substring(1);
            for (int i = 0, j = LunarUtil.ZHI.Length; i < j; i++) {
                if (LunarUtil.ZHI[i].Equals(timeZhi)) {
                    hour = (i - 1) * 2;
                }
            }
            while (startYear >= SolarUtil.BASE_YEAR - 1) {
                int year = startYear - 1;
                int counter = 0;
                int month = 12;
                int day;
                bool found = false;
                while (counter < 15) {
                    if (year >= SolarUtil.BASE_YEAR) {
                        day = 1;
                        if (year == SolarUtil.BASE_YEAR && month == SolarUtil.BASE_MONTH) {
                            day = SolarUtil.BASE_DAY;
                        }
                        Solar solar = Solar.fromYmdHms(year, month, day, hour, 0, 0);
                        lunar = solar.GetLunar();
                        if (lunar.GetYearInGanZhiExact().Equals(yearGanZhi) && lunar.GetMonthInGanZhiExact().Equals(monthGanZhi)) {
                            found = true;
                            break;
                        }
                    }
                    month++;
                    if (month > 12) {
                        month = 1;
                        year++;
                    }
                    counter++;
                }
                if (found) {
                    counter = 0;
                    month--;
                    if (month < 1) {
                        month = 12;
                        year--;
                    }
                    day = 1;
                    if (year == SolarUtil.BASE_YEAR && month == SolarUtil.BASE_MONTH) {
                        day = SolarUtil.BASE_DAY;
                    }
                    Solar solar = Solar.fromYmdHms(year, month, day, hour, 0, 0);
                    while (counter < 61) {
                        lunar = solar.GetLunar();
                        string dgz = (2 == sect) ? lunar.GetDayInGanZhiExact2() : lunar.GetDayInGanZhiExact();
                        if (lunar.GetYearInGanZhiExact().Equals(yearGanZhi) && lunar.GetMonthInGanZhiExact().Equals(monthGanZhi) && dgz.Equals(dayGanZhi) && lunar.GetTimeInGanZhi().Equals(timeGanZhi)) {
                            l.Add(solar);
                            break;
                        }
                        solar = solar.next(1);
                        counter++;
                    }
                }
                startYear -= 60;
            }
            return l;
        }

        public static Solar fromDate(DateTime date) {
            return new Solar(date);
        }

        public static Solar fromJulianDay(double julianDay) {
            return new Solar(julianDay);
        }

        public static Solar fromYmd(int year, int month, int day) {
            return new Solar(year, month, day);
        }


        public static Solar fromYmdHms(int year, int month, int day, int hour, int minute, int second) {
            return new Solar(year, month, day, hour, minute, second);
        }
        public Boolean isLeapYear() {
            return SolarUtil.IsLeapYear(year);
        }


        public int getWeek() {
            return Convert.ToInt32(calendar.DayOfWeek.ToString("d"));
        }


        public string getWeekInChinese() {
            return SolarUtil.WEEK[getWeek()];
        }

        public string getXingZuo() {
            int index = 11;
            int y = month * 100 + day;
            if (y >= 321 && y <= 419) {
                index = 0;
            } else if (y >= 420 && y <= 520) {
                index = 1;
            } else if (y >= 521 && y <= 621) {
                index = 2;
            } else if (y >= 622 && y <= 722) {
                index = 3;
            } else if (y >= 723 && y <= 822) {
                index = 4;
            } else if (y >= 823 && y <= 922) {
                index = 5;
            } else if (y >= 923 && y <= 1023) {
                index = 6;
            } else if (y >= 1024 && y <= 1122) {
                index = 7;
            } else if (y >= 1123 && y <= 1221) {
                index = 8;
            } else if (y >= 1222 || y <= 119) {
                index = 9;
            } else if (y <= 218) {
                index = 10;
            }
            return SolarUtil.XINGZUO[index];
        }


        public List<string> getFestivals() {
            List<string> l = new List<string>();
            //获取几月几日对应的节日
            try {
                l.Add(SolarUtil.FESTIVAL[month + "-" + day]);
            } catch { }
            //计算几月第几个星期几对应的节日
            int weeks = (int)Math.Ceiling(day / 7D);
            //星期几，0代表星期天
            int week = getWeek();
            try {
                l.Add(SolarUtil.WEEK_FESTIVAL[month + "-" + weeks + "-" + week]);
            } catch { }
            return l;
        }

        public List<string> getOtherFestivals() {
            List<string> l = new List<string>();
            try {
                List<string> fs = SolarUtil.OTHER_FESTIVAL[month + "-" + day];
                l.AddRange(fs);
            } catch { }
            return l;
        }

        public int getYear() {
            return year;
        }

        public int getMonth() {
            return month;
        }

        public int getDay() {
            return day;
        }

        public int getHour() {
            return hour;
        }

        public int getMinute() {
            return minute;
        }

        public int getSecond() {
            return second;
        }

        public DateTime getCalendar() {
            return calendar;
        }

        public Lunar GetLunar() {
            return new Lunar(calendar);
        }

        public double getJulianDay() {
            int y = this.year;
            int m = this.month;
            double d = this.day + ((this.second * 1D / 60 + this.minute) / 60 + this.hour) / 24;
            int n = 0;
            bool g = false;
            if (y * 372 + m * 31 + (int)d >= 588829) {
                g = true;
            }
            if (m <= 2) {
                m += 12;
                y--;
            }
            if (g) {
                n = (int)(y * 1D / 100);
                n = 2 - n + (int)(n * 1D / 4);
            }
            return (int)(365.25 * (y + 4716)) + (int)(30.6001 * (m + 1)) + d + n - 1524.5;
        }


        public override string ToString() {
            return toYmd();
        }

        public string toYmd() {
            return year + "-" + (month < 10 ? "0" : "") + month + "-" + (day < 10 ? "0" : "") + day;
        }


        public string toYmdHms() {
            return toYmd() + " " + (hour < 10 ? "0" : "") + hour + ":" + (minute < 10 ? "0" : "") + minute + ":" + (second < 10 ? "0" : "") + second;
        }

        public string ToFullString() {
            StringBuilder s = new StringBuilder();
            s.Append(toYmdHms());
            if (isLeapYear()) {
                s.Append(" ");
                s.Append("闰年");
            }
            s.Append(" ");
            s.Append("星期");
            s.Append(getWeekInChinese());

            s.Append(" ");
            s.Append(getXingZuo());
            s.Append("座");
            return s.ToString();
        }

        public Solar next(int days) {
            return next(days, false);
        }


        public Solar next(int days, bool onlyWorkday) {
            DateTime c = new DateTime(year, month, day, hour, minute, second);
            if (0 != days) {
                if (!onlyWorkday) {
                    c = c.AddDays(days);
                } else {
                    int rest = Math.Abs(days);
                    int add = days < 1 ? -1 : 1;
                    while (rest > 0) {
                        c = c.AddDays(add);
                        bool work = true;
                        Holiday holiday = HolidayUtil.getHoliday(c.Year, c.Month, c.Day);
                        if (null == holiday) {
                            string week = c.DayOfWeek.ToString("d");
                            if ("0".Equals(week) || "6".Equals(week)) {
                                work = false;
                            }
                        } else {
                            work = holiday.IsWork();
                        }
                        if (work) {
                            rest--;
                        }
                    }
                }
            }
            return new Solar(c);
        }
    }
}
