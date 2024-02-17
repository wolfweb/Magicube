namespace Magicube.Tools.Date {
    public class JieQi {
        private string _name;
        public string Name  { get {
                return _name;
            } set { 
                foreach(var key in LunarUtil.JIE) {
                    if (key == value) Jie = true;
                }
                if (!Jie) {
                    foreach(var key in LunarUtil.QI) {
                        if (key == value) Qi = true;
                    }
                }
                _name = value;
            } 
        }

        public Solar  Solar { get; }

        public bool   Jie   { get; set; }

        public bool   Qi    { get; set; }

        public JieQi() {
        }

        public JieQi(string name, Solar solar) {
            Name  = name;
            Solar = solar;
        }

        public override string ToString() {
            return Name;
        }

    }
}
