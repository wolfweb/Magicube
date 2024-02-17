namespace Magicube.Tools.Date {
    public class ShuJiu {     
        private string Name { get; set; }
        private int   Index { get; set; }
        public ShuJiu() {
        }

        public ShuJiu(string name, int index) {
            Name = name;
            Index = index;

        }

        public string ToFullString() {
            return Name + "第" + Index + "天";
        }

        public override string ToString() {
            return Name;
        }
    }
}
