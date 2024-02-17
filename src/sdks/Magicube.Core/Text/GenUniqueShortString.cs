using System;

namespace Magicube.Core.Text {
    public class GenUniqueShortString {
        static char[] sc;
        static DateTime startTime;
        static long prve = 0;
        static readonly object _lock = new object();

        public GenUniqueShortString() {
            if (sc == null) {
                string scString = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ$";
                sc = scString.ToCharArray();
                startTime = new DateTime(2000, 1, 1, 0, 0, 0, 0);
            }
        }

        public string Create() {
            lock (_lock) {
                TimeSpan ts = DateTime.UtcNow - startTime;
                long temp = Convert.ToInt64(ts.TotalMilliseconds * 10);
                if (temp > prve) {
                    prve = temp;
                    return ToShortString(temp);
                } else {
                    prve++;
                    return ToShortString(prve);
                }
            }
        }

        private string ToShortString(long num) {
            string str = "";
            while (num >= sc.Length) {
                str = sc[num % sc.Length] + str;
                num = num / sc.Length;
            }
            return sc[num] + str;
        }
    }
}
