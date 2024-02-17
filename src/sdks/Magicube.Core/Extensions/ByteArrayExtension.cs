using System;
using System.Text;

namespace Magicube.Core {
    public static class ByteArrayExtension {
        public static string ToBase64(this byte[] v) => Convert.ToBase64String(v);

        public static string ToHexString(this byte[] v) {
            return BitConverter.ToString(v).Replace("-", "");
        }

        public static string ToString(this byte[] v, string charSet = "utf-8") {
            try {
                return Encoding.GetEncoding(charSet).GetString(v);
            } catch {
				return Encoding.UTF8.GetString(v);
			}
        }
    }
}
