namespace Magicube.Tools.Date {
    public class Holiday {
        public string Day    { get; set; }

        public string Name   { get; set; }

        public bool   Work   { get; set; }

        public string Target { get; set; }

        public Holiday() { }

        public Holiday(string day, string name, bool work, string target) {
            if (!day.Contains("-")) {
                Day = day.Substring(0, 4) + "-" + day.Substring(4, 2) + "-" + day.Substring(6);
            } else {
                Day = day;
            }
            Name = name;
            Work = work;
            if (!target.Contains("-")) {
                Target = target.Substring(0, 4) + "-" + target.Substring(4, 2) + "-" + target.Substring(6);
            } else {
                Target = target;
            }
        }

        public bool IsWork() {
            return Work;
        }

        public override string ToString() {
            return Day + " " + Name + (Work ? "调休" : "") + " " + Target;
        }
    }
}
