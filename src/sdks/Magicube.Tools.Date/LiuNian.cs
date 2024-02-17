namespace Magicube.Tools.Date {
    public class LiuNian {
        public int   Index { get; }
        public DaYun DaYun { get; }
        public int   Year  { get; }
        public int   Age   { get; }
        public Lunar Lunar { get; }

        public LiuNian(DaYun daYun, int index) {
            DaYun = daYun;
            Lunar = daYun.Lunar;
            Index = index;
            Year  = daYun.StartYear + index;
            Age   = daYun.StartAge  + index;
        }

        public string GetGanZhi() {
            int offset = LunarUtil.getJiaZiIndex(Lunar.getJieQiTable()["立春"].GetLunar().GetYearInGanZhiExact()) + Index;
            if (DaYun.Index > 0) {
                offset += DaYun.StartAge - 1;
            }
            offset %= LunarUtil.JIA_ZI.Length;
            return LunarUtil.JIA_ZI[offset];
        }

        public string GetXun() {
            return LunarUtil.getXun(GetGanZhi());
        }

        public string GetXunKong() {
            return LunarUtil.getXunKong(GetGanZhi());
        }

        public LiuYue[] GetLiuYue() {
            int n = 12;
            LiuYue[] l = new LiuYue[n];
            for (int i = 0; i < n; i++) {
                l[i] = new LiuYue(this, i);
            }
            return l;
        }

    }
}
