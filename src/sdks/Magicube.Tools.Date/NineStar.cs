using System.Text;

namespace Magicube.Tools.Date {
    public class NineStar {
        public static readonly string[] NUMBER         = { "一", "二", "三", "四", "五", "六", "七", "八", "九" };

        public static readonly string[] COLOR          = { "白", "黒", "碧", "绿", "黄", "白", "赤", "白", "紫" };

        public static readonly string[] WU_XING        = { "水", "土", "木", "木", "土", "金", "金", "土", "火" };

        public static readonly string[] POSITION       = { "坎", "坤", "震", "巽", "中", "乾", "兑", "艮", "离" };

        public static readonly string[] NAME_BEI_DOU   = { "天枢", "天璇", "天玑", "天权", "玉衡", "开阳", "摇光", "洞明", "隐元" };

        public static readonly string[] NAME_XUAN_KONG = { "贪狼", "巨门", "禄存", "文曲", "廉贞", "武曲", "破军", "左辅", "右弼" };

        public static readonly string[] NAME_QI_MEN    = { "天蓬", "天芮", "天冲", "天辅", "天禽", "天心", "天柱", "天任", "天英" };

        public static readonly string[] BA_MEN_QI_MEN  = { "休", "死", "伤", "杜", "", "开", "惊", "生", "景" };

        public static readonly string[] NAME_TAI_YI    = { "太乙", "摄提", "轩辕", "招摇", "天符", "青龙", "咸池", "太阴", "天乙" };

        public static readonly string[] TYPE_TAI_YI    = { "吉神", "凶神", "安神", "安神", "凶神", "吉神", "凶神", "吉神", "吉神" };

        public static readonly string[] SONG_TAI_YI = { "门中太乙明，星官号贪狼，赌彩财喜旺，婚姻大吉昌，出入无阻挡，参谒见贤良，此行三五里，黑衣别阴阳。", "门前见摄提，百事必忧疑，相生犹自可，相克祸必临，死门并相会，老妇哭悲啼，求谋并吉事，尽皆不相宜，只可藏隐遁，若动伤身疾。", "出入会轩辕，凡事必缠牵，相生全不美，相克更忧煎，远行多不利，博彩尽输钱，九天玄女法，句句不虚言。", "招摇号木星，当之事莫行，相克行人阻，阴人口舌迎，梦寐多惊惧，屋响斧自鸣，阴阳消息理，万法弗违情。", "五鬼为天符，当门阴女谋，相克无好事，行路阻中途，走失难寻觅，道逢有尼姑，此星当门值，万事有灾除。", "神光跃青龙，财气喜重重，投入有酒食，赌彩最兴隆，更逢相生旺，休言克破凶，见贵安营寨，万事总吉同。", "吾将为咸池，当之尽不宜，出入多不利，相克有灾情，赌彩全输尽，求财空手回，仙人真妙语，愚人莫与知，动用虚惊退，反复逆风吹。", "坐临太阴星，百祸不相侵，求谋悉成就，知交有觅寻，回风归来路，恐有殃伏起，密语中记取，慎乎莫轻行。", "迎来天乙星，相逢百事兴，运用和合庆，茶酒喜相迎，求谋并嫁娶，好合有天成，祸福如神验，吉凶甚分明。" };
                
        public static readonly string[] LUCK_XUAN_KONG = { "吉", "凶", "凶", "吉", "凶", "吉", "凶", "吉", "吉" };

        public static readonly string[] LUCK_QI_MEN = { "大凶", "大凶", "小吉", "大吉", "大吉", "大吉", "小凶", "小吉", "小凶" };

        public static readonly string[] YIN_YANG_QI_MEN = { "阳", "阴", "阳", "阳", "阳", "阴", "阴", "阳", "阴" };

        protected int index;

        public NineStar(int index) {
            this.index = index;
        }

        public static NineStar fromIndex(int index) {
            return new NineStar(index);
        }

        public string getNumber() {
            return NUMBER[index];
        }

        public string getColor() {
            return COLOR[index];
        }

        public string getWuXing() {
            return WU_XING[index];
        }

        public string getPosition() {
            return POSITION[index];
        }

        public string getPositionDesc() {
            return LunarUtil.POSITION_DESC[getPosition()];
        }

        public string getNameInXuanKong() {
            return NAME_XUAN_KONG[index];
        }

        public string getNameInBeiDou() {
            return NAME_BEI_DOU[index];
        }

        public string getNameInQiMen() {
            return NAME_QI_MEN[index];
        }

        public string getNameInTaiYi() {
            return NAME_TAI_YI[index];
        }

        public string getLuckInQiMen() {
            return LUCK_QI_MEN[index];
        }

        public string getLuckInXuanKong() {
            return LUCK_XUAN_KONG[index];
        }

        public string getYinYangInQiMen() {
            return YIN_YANG_QI_MEN[index];
        }

        public string getTypeInTaiYi() {
            return TYPE_TAI_YI[index];
        }

        public string getBaMenInQiMen() {
            return BA_MEN_QI_MEN[index];
        }

        public string getSongInTaiYi() {
            return SONG_TAI_YI[index];
        }

        public int getIndex() {
            return index;
        }

        public override string ToString() {
            return toString();
        }

        public string toString() {
            return getNumber() + getColor() + getWuXing() + getNameInBeiDou();
        }

        public string toFullString() {
            StringBuilder s = new StringBuilder();
            s.Append(getNumber());
            s.Append(getColor());
            s.Append(getWuXing());
            s.Append(" ");
            s.Append(getPosition());
            s.Append("(");
            s.Append(getPositionDesc());
            s.Append(") ");
            s.Append(getNameInBeiDou());
            s.Append(" 玄空[");
            s.Append(getNameInXuanKong());
            s.Append(" ");
            s.Append(getLuckInXuanKong());
            s.Append("] 奇门[");
            s.Append(getNameInQiMen());
            s.Append(" ");
            s.Append(getLuckInQiMen());
            if (getBaMenInQiMen().Length > 0) {
                s.Append(" ");
                s.Append(getBaMenInQiMen());
                s.Append("门");
            }
            s.Append(" ");
            s.Append(getYinYangInQiMen());
            s.Append("] 太乙[");
            s.Append(getNameInTaiYi());
            s.Append(" ");
            s.Append(getTypeInTaiYi());
            s.Append("]");
            return s.ToString();
        }
    }
}
