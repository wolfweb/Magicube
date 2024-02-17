﻿using System.Collections.Generic;

namespace Magicube.Tools.Date {

    public class EightChar {
        
        public static readonly string[] MONTH_ZHI   = { "", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥", "子", "丑" };
        
        public static readonly string[] CHANG_SHENG = { "长生", "沐浴", "冠带", "临官", "帝旺", "衰", "病", "死", "墓", "绝", "胎", "养" };

        private static readonly Dictionary<string, int> CHANG_SHENG_OFFSET = new Dictionary<string, int>();

        static EightChar() {
            //阳
            CHANG_SHENG_OFFSET.Add("甲", 1);
            CHANG_SHENG_OFFSET.Add("丙", 10);
            CHANG_SHENG_OFFSET.Add("戊", 10);
            CHANG_SHENG_OFFSET.Add("庚", 7);
            CHANG_SHENG_OFFSET.Add("壬", 4);
            //阴
            CHANG_SHENG_OFFSET.Add("乙", 6);
            CHANG_SHENG_OFFSET.Add("丁", 9);
            CHANG_SHENG_OFFSET.Add("己", 9);
            CHANG_SHENG_OFFSET.Add("辛", 0);
            CHANG_SHENG_OFFSET.Add("癸", 3);
        }

        public int Sect { get; set; } = 2;

        private Lunar lunar;

        public EightChar(Lunar lunar) {
            this.lunar = lunar;
        }

        public static EightChar fromLunar(Lunar lunar) {
            return new EightChar(lunar);
        }

        public override string ToString() {
            return getYear() + " " + getMonth() + " " + getDay() + " " + getTime();
        }

        public string getYear() {
            return lunar.GetYearInGanZhiExact();
        }

        public string getYearGan() {
            return lunar.GetYearGanExact();
        }

        public string getYearZhi() {
            return lunar.GetYearZhiExact();
        }

        public List<string> getYearHideGan() {
            return LunarUtil.ZHI_HIDE_GAN[getYearZhi()];
        }

        public string getYearWuXing() {
            return LunarUtil.WU_XING_GAN[getYearGan()] + LunarUtil.WU_XING_ZHI[getYearZhi()];
        }

        public string getYearNaYin() {
            return LunarUtil.NAYIN[getYear()];
        }

        public string getYearShiShenGan() {
            return LunarUtil.SHI_SHEN_GAN[getDayGan() + getYearGan()];
        }

        private List<string> getShiShenZhi(string zhi) {
            List<string> hideGan = LunarUtil.ZHI_HIDE_GAN[zhi];
            List<string> l = new List<string>(hideGan.Count);
            foreach (string gan in hideGan) {
                l.Add(LunarUtil.SHI_SHEN_ZHI[getDayGan() + zhi + gan]);
            }
            return l;
        }

        public List<string> getYearShiShenZhi() {
            return getShiShenZhi(getYearZhi());
        }

        public int getDayGanIndex() {
            return (2 == Sect) ? lunar.DayGanIndexExact2 : lunar.DayGanIndexExact;
        }

        public int getDayZhiIndex() {
            return (2 == Sect) ? lunar.DayZhiIndexExact2 : lunar.DayZhiIndexExact;
        }

        private string getDiShi(int zhiIndex) {
            int offset = CHANG_SHENG_OFFSET[getDayGan()];
            int index = offset + (getDayGanIndex() % 2 == 0 ? zhiIndex : -zhiIndex);
            if (index >= 12) {
                index -= 12;
            }
            if (index < 0) {
                index += 12;
            }
            return CHANG_SHENG[index];
        }

        public string getYearDiShi() {
            return getDiShi(lunar.YearZhiIndexExact);
        }

        public string getMonth() {
            return lunar.GetMonthInGanZhiExact();
        }

        public string getMonthGan() {
            return lunar.GetMonthGanExact();
        }

        public string getMonthZhi() {
            return lunar.GetMonthZhiExact();
        }

        public List<string> getMonthHideGan() {
            return LunarUtil.ZHI_HIDE_GAN[getMonthZhi()];
        }

        public string getMonthWuXing() {
            return LunarUtil.WU_XING_GAN[getMonthGan()] + LunarUtil.WU_XING_ZHI[getMonthZhi()];
        }

        public string getMonthNaYin() {
            return LunarUtil.NAYIN[getMonth()];
        }

        public string getMonthShiShenGan() {
            return LunarUtil.SHI_SHEN_GAN[getDayGan() + getMonthGan()];
        }

        public List<string> getMonthShiShenZhi() {
            return getShiShenZhi(getMonthZhi());
        }

        public string getMonthDiShi() {
            return getDiShi(lunar.MonthZhiIndexExact);
        }

        public string getDay() {
            return (2 == Sect) ? lunar.GetDayInGanZhiExact2() : lunar.GetDayInGanZhiExact();
        }

        public string getDayGan() {
            return (2 == Sect) ? lunar.GetDayGanExact2() : lunar.GetDayGanExact();
        }

        public string getDayZhi() {
            return (2 == Sect) ? lunar.GetDayZhiExact2() : lunar.GetDayZhiExact();
        }

        public List<string> getDayHideGan() {
            return LunarUtil.ZHI_HIDE_GAN[getDayZhi()];
        }

        public string getDayWuXing() {
            return LunarUtil.WU_XING_GAN[getDayGan()] + LunarUtil.WU_XING_ZHI[getDayZhi()];
        }

        public string getDayNaYin() {
            return LunarUtil.NAYIN[getDay()];
        }

        public string getDayShiShenGan() {
            return "日主";
        }

        public List<string> getDayShiShenZhi() {
            return getShiShenZhi(getDayZhi());
        }

        public string getDayDiShi() {
            return getDiShi(getDayZhiIndex());
        }

        public string getTime() {
            return lunar.GetTimeInGanZhi();
        }

        public string getTimeGan() {
            return lunar.GetTimeGan();
        }

        public string getTimeZhi() {
            return lunar.GetTimeZhi();
        }

        public List<string> getTimeHideGan() {
            return LunarUtil.ZHI_HIDE_GAN[getTimeZhi()];
        }

        public string getTimeWuXing() {
            return LunarUtil.WU_XING_GAN[lunar.GetTimeGan()] + LunarUtil.WU_XING_ZHI[lunar.GetTimeZhi()];
        }

        public string getTimeNaYin() {
            return LunarUtil.NAYIN[getTime()];
        }

        public string getTimeShiShenGan() {
            return LunarUtil.SHI_SHEN_GAN[getDayGan() + getTimeGan()];
        }

        public List<string> getTimeShiShenZhi() {
            return getShiShenZhi(getTimeZhi());
        }

        public string getTimeDiShi() {
            return getDiShi(lunar.TimeZhiIndex);
        }

        public string getTaiYuan() {
            int ganIndex = lunar.MonthGanIndexExact + 1;
            if (ganIndex >= 10) {
                ganIndex -= 10;
            }
            int zhiIndex = lunar.MonthZhiIndexExact + 3;
            if (zhiIndex >= 12) {
                zhiIndex -= 12;
            }
            return LunarUtil.GAN[ganIndex + 1] + LunarUtil.ZHI[zhiIndex + 1];
        }

        public string getTaiYuanNaYin() {
            return LunarUtil.NAYIN[getTaiYuan()];
        }

        public string getMingGong() {
            int monthZhiIndex = 0;
            int timeZhiIndex = 0;
            for (int i = 0, j = MONTH_ZHI.Length; i < j; i++) {
                string zhi = MONTH_ZHI[i];
                if (lunar.GetMonthZhiExact().Equals(zhi)) {
                    monthZhiIndex = i;
                }
                if (lunar.GetTimeZhi().Equals(zhi)) {
                    timeZhiIndex = i;
                }
            }
            int zhiIndex = 26 - (monthZhiIndex + timeZhiIndex);
            if (zhiIndex > 12) {
                zhiIndex -= 12;
            }
            int jiaZiIndex = LunarUtil.getJiaZiIndex(lunar.GetMonthInGanZhiExact()) - (monthZhiIndex - zhiIndex);
            if (jiaZiIndex >= 60) {
                jiaZiIndex -= 60;
            }
            if (jiaZiIndex < 0) {
                jiaZiIndex += 60;
            }
            return LunarUtil.JIA_ZI[jiaZiIndex];
        }

        public string getMingGongNaYin() {
            return LunarUtil.NAYIN[getMingGong()];
        }

        public string getShenGong() {
            int monthZhiIndex = 0;
            int timeZhiIndex = 0;
            for (int i = 0, j = MONTH_ZHI.Length; i < j; i++) {
                string zhi = MONTH_ZHI[i];
                if (lunar.GetMonthZhiExact().Equals(zhi)) {
                    monthZhiIndex = i;
                }
                if (lunar.GetTimeZhi().Equals(zhi)) {
                    timeZhiIndex = i;
                }
            }
            int zhiIndex = (2 + (monthZhiIndex + timeZhiIndex)) % 12;
            int jiaZiIndex = LunarUtil.getJiaZiIndex(lunar.GetMonthInGanZhiExact()) - (monthZhiIndex - zhiIndex);
            if (jiaZiIndex >= 60) {
                jiaZiIndex -= 60;
            }
            if (jiaZiIndex < 0) {
                jiaZiIndex += 60;
            }
            return LunarUtil.JIA_ZI[jiaZiIndex];
        }

        public string getShenGongNaYin() {
            return LunarUtil.NAYIN[getShenGong()];
        }

        public Lunar getLunar() {
            return lunar;
        }

        public Yun getYun(int gender) {
            return new Yun(this, gender);
        }

        public string getYearXun() {
            return lunar.getYearXunExact();
        }

        public string getYearXunKong() {
            return lunar.getYearXunKongExact();
        }

        public string getMonthXun() {
            return lunar.getMonthXunExact();
        }

        public string getMonthXunKong() {
            return lunar.getMonthXunKongExact();
        }

        public string getDayXun() {
            return (2 == Sect) ? lunar.getDayXunExact2() : lunar.getDayXunExact();
        }

        public string getDayXunKong() {
            return (2 == Sect) ? lunar.getDayXunKongExact2() : lunar.getDayXunKongExact();
        }

        public string getTimeXun() {
            return lunar.getTimeXun();
        }

        public string getTimeXunKong() {
            return lunar.getTimeXunKong();
        }

    }
}
