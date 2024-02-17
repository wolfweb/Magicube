namespace Magicube.Tools.Date {
    public class XiaoYun {             
        public int   Index   { get; }        
        public DaYun DaYun   { get; }
        public int   Year    { get; }
        public int   Age     { get; }
        public bool  Forward { get; }
        public Lunar Lunar   { get; }

        public XiaoYun(DaYun daYun, int index, bool forward) {
            DaYun   = daYun;
            Lunar   = daYun.Lunar;
            Index   = index;
            Year    = daYun.StartYear + index;
            Age     = daYun.StartAge + index;
            Forward = forward;
        }

        public string GetGanZhi() {
            int offset = LunarUtil.getJiaZiIndex(Lunar.GetTimeInGanZhi());
            int add = this.Index + 1;
            if (DaYun.Index > 0) {
                add += DaYun.StartAge - 1;
            }
            offset += Forward ? add : -add;
            int size = LunarUtil.JIA_ZI.Length;
            while (offset < 0) {
                offset += size;
            }
            offset %= size;
            return LunarUtil.JIA_ZI[offset];
        }

        public string GetXun() {
            return LunarUtil.getXun(GetGanZhi());
        }

        public string GetXunKong() {
            return LunarUtil.getXunKong(GetGanZhi());
        }
    }
}
