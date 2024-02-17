using System;

namespace Magicube.LightApp.Wechat {
    public static class StringExtension {
        public static string AsUrlData(this string data) {
            if (data == null) {
                return null;
            }
            return Uri.EscapeDataString(data);
        }
    }
}
