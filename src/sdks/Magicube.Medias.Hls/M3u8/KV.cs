using System;
using System.Collections.Generic;

namespace Magicube.Medias.Hls {
    public static class KV {
        public static KeyValuePair<string, string> Parse(string text, char separator = ':') {
            if (string.IsNullOrEmpty(text))
                return new KeyValuePair<string, string>();

            var strArray = text.Split(
                new[] { separator }, 2, StringSplitOptions.RemoveEmptyEntries
            );

            if (strArray.Length == 2) {
                return new KeyValuePair<string, string>(strArray[0].Trim(),
                    (strArray.Length > 1 ? strArray[1] : string.Empty).Trim(' ', '"'));
            }

            return new KeyValuePair<string, string>();
        }
    }
}