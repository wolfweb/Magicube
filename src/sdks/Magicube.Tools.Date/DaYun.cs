namespace Magicube.Tools.Date {
    public class DaYun {        
        public  int  StartYear { get; }                      
        public int   EndYear   { get; }                                
        public  int  StartAge  { get; }
        public int   EndAge    { get; }
        public int   Index     { get; }
        public Lunar Lunar     { get; }

                
        private readonly Yun _yun;

        public DaYun(Yun yun, int index) {
            _yun = yun;
            Lunar = yun.Lunar;
            Index = index;
            int birthYear = yun.Lunar.Solar.getYear();
            int year = yun.GetStartSolar().getYear();
            if (index < 1) {
                StartYear = birthYear;
                StartAge  = 1;
                EndYear   = year - 1;
                EndAge    = year - birthYear;
            } else {
                int add = (index - 1) * 10;
                StartYear = year + add;
                StartAge  = StartYear - birthYear + 1;
                EndYear   = StartYear + 9;
                EndAge    = StartAge + 9;
            }
        }


        public string GetGanZhi() {
            if (Index < 1) {
                return "";
            }
            int offset = LunarUtil.getJiaZiIndex(Lunar.GetMonthInGanZhiExact());
            offset += _yun.Forward ? Index : -Index;
            int size = LunarUtil.JIA_ZI.Length;
            if (offset >= size) {
                offset -= size;
            }
            if (offset < 0) {
                offset += size;
            }
            return LunarUtil.JIA_ZI[offset];
        }

        public string GetXun() {
            return LunarUtil.getXun(GetGanZhi());
        }

        public string GetXunKong() {
            return LunarUtil.getXunKong(GetGanZhi());
        }

        public LiuNian[] GetLiuNian() {
            int n = 10;
            if (Index < 1) {
                n = EndYear - StartYear + 1;
            }
            LiuNian[] l = new LiuNian[n];
            for (int i = 0; i < n; ++i) {
                l[i] = new LiuNian(this, i);
            }
            return l;
        }

        public XiaoYun[] GetXiaoYun() {
            int n = 10;
            if (Index < 1) {
                n = EndYear - StartYear + 1;
            }
            XiaoYun[] l = new XiaoYun[n];
            for (int i = 0; i < n; ++i) {
                l[i] = new XiaoYun(this, i, _yun.Forward);
            }
            return l;
        }

    }
}
