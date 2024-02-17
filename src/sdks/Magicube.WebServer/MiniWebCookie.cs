using System;
using System.Text;
using System.Globalization;

namespace Magicube.WebServer {
    public class MiniWebCookie {
        public MiniWebCookie(string name, string value, string path = "/", bool httpOnly = false, bool secure = false, DateTime? expires = null) {
            Name = name;
            Value = value;
            Path = path;
            HttpOnly = httpOnly;
            Secure = secure;
            Expires = expires;
        }

        public string Domain;

        public DateTime? Expires;

        public string Name;
        public string Path;
        public string Value;
        public bool   HttpOnly;
        public bool   Secure;
        public override string ToString() {
            StringBuilder sb = new StringBuilder(50).AppendFormat("{0}={1}; path={2}", Name, Value, Path);

            if (Expires != null) {
                sb.Append("; expires=");
                sb.Append(Expires.Value.ToUniversalTime().ToString("ddd, dd-MMM-yyyy HH:mm:ss", DateTimeFormatInfo.InvariantInfo));
                sb.Append(" GMT");
            }

            if (Domain != null)
                sb.Append("; domain=").Append(Domain);

            if (Secure)
                sb.Append("; Secure");

            if (HttpOnly)
                sb.Append("; HttpOnly");

            return sb.ToString();
        }
    }

}
