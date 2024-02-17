using Magicube.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.Text.Unicode {
    public static class UnicodeExtensions {
        public static IEnumerable<Codepoint> Codepoints(this string s) {
            for (int i = 0; i < s.Length; ++i) {
                if (char.IsHighSurrogate(s[i])) {
                    if (s.Length < i + 2) {
                        throw new InvalidEncodingException();
                    }
                    if (!char.IsLowSurrogate(s[i + 1])) {
                        throw new InvalidEncodingException();
                    }
                    yield return new Codepoint(char.ConvertToUtf32(s[i], s[++i]));
                } else {
                    yield return new Codepoint((int)s[i]);
                }
            }
        }

        public static IEnumerable<string> Letters(this string s) {
            for (int i = 0; i < s.Length; ++i) {
                if (Char.IsHighSurrogate(s[i])) {
                    if (s.Length < i + 2) {
                        throw new InvalidEncodingException();
                    }
                    if (!char.IsLowSurrogate(s[i + 1])) {
                        throw new InvalidEncodingException();
                    }
                    yield return "{0}{1}".Format(s[i], s[++i]);
                } else {
                    yield return "{0}".Format(s[i]);
                }
            }
        }

        public static UnicodeSequence AsUnicodeSequence(this string s) {
            return new UnicodeSequence(s.Codepoints());
        }

        public static bool In<T>(this T t, IEnumerable<T> collection) {
            return collection.Contains(t);
        }
    }
}
