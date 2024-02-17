namespace Magicube.Tools.Date {
    public class Fu {
        private string Name  { get; set; }

        private int    Index { get; set; }

        public Fu() {
        }

        public Fu(string name, int index) {
            Name  = name;
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
