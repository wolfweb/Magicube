using Magicube.Core.Runtime.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Magicube.Core {
#if !DEBUG
using System.Diagnostics;
    [DebuggerStepThrough]
#endif
    public static class StringExtension {
        private static readonly char[] ValidSegmentChars   = "/?#[]@\"^{}|`<>\t\r\n\f ".ToCharArray();
        private static readonly Regex  EmailExpression     = new Regex(@"^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$", RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private static readonly Regex  WebUrlExpression    = new Regex(@"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        public static bool     Any(this string subject, params char[] chars) {
            if (subject.IsNullOrEmpty() || chars == null || chars.Length == 0) {
                return false;
            }

            for (var i = 0; i < subject.Length; i++) {
                char current = subject[i];
                if (Array.IndexOf(chars, current) >= 0) {
                    return true;
                }
            }

            return false;
        }
                               
        [RuntimeMethod]
        public static bool     AsBool(this string v) {
            bool.TryParse(v, out bool result);
            return result;
        }

        [RuntimeMethod]
        public static DateTime AsDateTime(this string v) {
            DateTime.TryParse(v, out DateTime result);
            return result;
        }

        [RuntimeMethod]
        public static decimal  AsDecimal(this string v) {
            decimal.TryParse(v, out decimal result);
            return result;
        }

        [RuntimeMethod]
        public static int      AsInt(this string v) {
            int.TryParse(v, out int result);
            return result;
        }
                               
        [RuntimeMethod]
        public static long     AsLong(this string v) {
            long.TryParse(v, out long result);
            return result;
        }
                               
        [RuntimeMethod]
        public static string   CleanHtml(this string Htmlstring) {
            Htmlstring = Regex.Replace(Htmlstring, @"<script[\s\S]*?</script>", "", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Htmlstring = Regex.Replace(Htmlstring, @"<noscript[\s\S]*?</noscript>", "", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Htmlstring = Regex.Replace(Htmlstring, @"<style[\s\S]*?</style>", "", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Htmlstring = Regex.Replace(Htmlstring, @"<.*?>", "", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", " ", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", " ", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", " ", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", " ", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", " ", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return Htmlstring;
        }
                               
        public static bool     IsValidUrlSegment(this string v) {           
            return !v.Any(ValidSegmentChars);
        }
                               
        [RuntimeMethod]
        public static bool     IsNullOrEmpty(this string v) {
            return string.IsNullOrWhiteSpace(v);
        }
                               
        [RuntimeMethod]
        public static bool     IsEmail(this string v) {
            return !v.IsNullOrEmpty() && EmailExpression.IsMatch(v);
        }
                               
        [RuntimeMethod]
        public static bool     IsWebUrl(this string v) {
            return !v.IsNullOrEmpty() && WebUrlExpression.IsMatch(v);
        }
                               
        [RuntimeMethod]
        public static bool     IsInt(this string v) {
            int result;
            return int.TryParse(v, out result);
        }
                               
        [RuntimeMethod]
        public static bool     IsDateTime(this string v) {
            DateTime result;
            return DateTime.TryParse(v, out result);
        }
                               
        [RuntimeMethod]
        public static bool     IsFloat(this string v) {
            float result;
            return float.TryParse(v, out result);
        }

        [RuntimeMethod]
        public static bool     IsJson(this string v) {
            v = v.Trim();
            return (v.StartsWith("{") && v.EndsWith("}")) || (v.StartsWith("[") && v.EndsWith("]"));
        }

        public static string   NotNullOrEmpty(this string v, [CallerMemberName] string name = null) {
            if (v.IsNullOrEmpty()) {
                throw new ArgumentNullException(name);
            }

            return v;
        }

        public static byte[]   ToByte(this string v, string charSet = "utf-8") {
            try {
                return Encoding.GetEncoding(charSet).GetBytes(v);
            } catch {
                return Encoding.UTF8.GetBytes(v);
            }
        }
                               
        public static T        ToEnum<T>(this string v, T defaultValue) where T : struct, IComparable, IFormattable {
            T convertedValue = defaultValue;

            if (!v.IsNullOrEmpty() && !Enum.TryParse(v.Trim(), true, out convertedValue)) {
                convertedValue = defaultValue;
            }

            return convertedValue;
        }
                               
        public static T        ToEnum<T>(this int v, T defaultValue) where T : struct, IComparable, IFormattable {
            T convertedValue;

            if (!Enum.TryParse(v.ToString(), true, out convertedValue)) {
                convertedValue = defaultValue;
            }

            return convertedValue;
        }

        public static byte[]   ToHex(this string v) {
            v = v.StartsWith("0x", StringComparison.Ordinal) ? v[2..] : v;
            return Convert.FromHexString(v);
        }

        [RuntimeMethod]
        public static string   ToSnakeCase(this string v) {
            if (v.IsNullOrEmpty()) {
                return v;
            }

            var sb = new StringBuilder();
            var state = SnakeCaseState.Start;

            for (var i = 0; i < v.Length; i++) {
                if (v[i] == ' ') {
                    if (state != SnakeCaseState.Start) {
                        state = SnakeCaseState.NewWord;
                    }
                } else if (char.IsUpper(v[i])) {
                    switch (state) {
                        case SnakeCaseState.Upper:
                            var hasNext = i + 1 < v.Length;
                            if (i > 0 && hasNext) {
                                var nextChar = v[i + 1];
                                if (!char.IsUpper(nextChar) && nextChar != '_') {
                                    sb.Append('_');
                                }
                            }
                            break;
                        case SnakeCaseState.Lower:
                        case SnakeCaseState.NewWord:
                            sb.Append('_');
                            break;
                    }

                    sb.Append(char.ToLowerInvariant(v[i]));

                    state = SnakeCaseState.Upper;
                } else if (v[i] == '_') {
                    sb.Append('_');
                    state = SnakeCaseState.Start;
                } else {
                    if (state == SnakeCaseState.NewWord) {
                        sb.Append('_');
                    }

                    sb.Append(v[i]);
                    state = SnakeCaseState.Lower;
                }
            }

            return sb.ToString();
        }

        [RuntimeMethod]
        public static string   ToCamelCase(this string v) {
            if (v.IsNullOrEmpty() || !char.IsUpper(v[0])) {
                return v;
            }

            var chars = v.ToCharArray();

            for (var i = 0; i < chars.Length; i++) {
                if (i == 1 && !char.IsUpper(chars[i])) {
                    break;
                }

                var hasNext = i + 1 < chars.Length;
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1])) {
                    if (char.IsSeparator(chars[i + 1])) {
                        chars[i] = char.ToLowerInvariant(chars[i]);
                    }

                    break;
                }

                chars[i] = char.ToLowerInvariant(chars[i]);
            }

            return new string(chars);
        }

        [RuntimeMethod]
        public static string   ToPascalCase(this string str) {
            IList<char> list = new List<char>();
            if (str.Length > 0) {
                list.Add(char.ToUpper(str[0]));
            }
            for (var i=1; i< str.Length;i++) {
                if (char.IsWhiteSpace(str[i]) || str[i] == '_') continue;
                if (char.IsWhiteSpace(str[i - 1]) || str[i - 1] == '_') {
                    list.Add(char.ToUpper(str[i]));
                } else {
                    list.Add(char.ToLower(str[i]));
                }
            }
            return new string(list.ToArray());
        }

        [RuntimeMethod]
        public static string   ToFriendly(this string v) {
            return Regex.Replace(v, @"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])", " ", RegexOptions.Compiled);
        }
                               
        [RuntimeMethod]
        public static string   ToUnicode(this string v) {
            if (v.IsNullOrEmpty()) return v;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < v.Length; i++) {
                builder.Append("\\u" + ((int)v[i]).ToString("x"));
            }
            return builder.ToString();
        }

        [RuntimeMethod]
        public static string   TrimStart(this string v, string prefix) {
            if (string.IsNullOrEmpty(prefix)) return v;

            string result = v;
            while (result.StartsWith(prefix)) {
                result = result.Substring(prefix.Length);
            }

            return result;
        }

        [RuntimeMethod]
        public static string   TrimEnd(this string v, string subfix) {
            if (string.IsNullOrEmpty(subfix)) return v;

            string result = v;
            while (result.EndsWith(subfix)) {
                result = result.Substring(0, result.Length - subfix.Length);
            }

            return result;
        }

        public static byte[]   FromBase64(this string v) {
            return Convert.FromBase64String(v);
        }

        public static byte[]   Base64UrlDecode(this string v) {
            string s = v;
            s = s.Replace('-', '+');
            s = s.Replace('_', '/');
            switch (s.Length % 4) {
                case 0: break;
                case 2: s += "=="; break;
                case 3: s += "="; break;
                default: throw new System.Exception("No es una cadena base64 válida");
            }
            return Convert.FromBase64String(s);
        }

        public static T        JsonToObject<T>(this string v) {
            if (v.IsNullOrEmpty()) return default(T);
            if ((v.StartsWith("{") && v.EndsWith("}")) || (v.StartsWith("[") && v.EndsWith("]"))) {
                return Json.Parse<T>(v);
            }
            return default(T);
        }

        [RuntimeMethod]
        public static string[] ToArray(this string value, string splitChars = ",，;；") {
            if (value.IsNullOrEmpty()) return Enumerable.Empty<string>().ToArray();

            return value.Split(splitChars.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        public static string   SubString(this string value, int length, string tail = "...") {
            if (string.IsNullOrEmpty(value) || value.Length <= length) {
                return value;
            }

            int len = 0;
            int i = 0;

            while (len < length && i < value.Length) {
                if (char.IsHighSurrogate(value[i])) {
                    length += 2;
                    i += 2;
                } else {
                    length += 1;
                    i += 1;
                }
            }
            return $"{value.Substring(0, i)}{tail}";
        }

        enum SnakeCaseState {
            Start,
            Lower,
            Upper,
            NewWord
        }
    }
}
