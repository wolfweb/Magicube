namespace Magicube.Tools.Date {
    public class LiuYue {
        private readonly LiuNian _liuNian;

        public int Index { get; }

        public LiuYue(LiuNian liuNian, int index) {
            _liuNian = liuNian;
            Index    = index;
        }

        public string GetMonthInChinese() {
            return LunarUtil.MONTH[Index + 1];
        }

        public string GetGanZhi() {
            int offset = 0;
            string yearGan = _liuNian.GetGanZhi().Substring(0, 1);
            if ("甲".Equals(yearGan) || "己".Equals(yearGan)) {
                offset = 2;
            } else if ("乙".Equals(yearGan) || "庚".Equals(yearGan)) {
                offset = 4;
            } else if ("丙".Equals(yearGan) || "辛".Equals(yearGan)) {
                offset = 6;
            } else if ("丁".Equals(yearGan) || "壬".Equals(yearGan)) {
                offset = 8;
            }
            string gan = LunarUtil.GAN[(Index + offset) % 10 + 1];
            string zhi = LunarUtil.ZHI[(Index + LunarUtil.BASE_MONTH_ZHI_INDEX) % 12 + 1];
            return gan + zhi;
        }

        public string GetXun() {
            return LunarUtil.getXun(GetGanZhi());
        }

        public string GetXunKong() {
            return LunarUtil.getXunKong(GetGanZhi());
        }

    }
}
