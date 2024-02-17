using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Magicube.Win {
    public class CommandLineParser {
        public CommandLineParserSetting Setting { get; set; }

        public CommandLineParser() {
            Setting = new CommandLineParserSetting(StartChar.Any, SplitChar.Any);
        }

        public CommandLineParser(CommandLineParserSetting setting) {
            Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> Parse(string[] args) {
            foreach (var arg in args) {
                if (!string.IsNullOrWhiteSpace(arg) && arg.Length > 1 && Setting.ValidStart(arg[0])) {
                    Int32 position = 1;
                    while (position < arg.Length && !Setting.ValidSplit(arg[position])) {
                        position++;
                    }
                    if (position > 1) {
                        string key = arg.Substring(1, position - 1);
                        string value = null;
                        if (position < arg.Length - 1) {
                            value = arg.Substring(position + 1, arg.Length - 1 - position);
                        }
                        yield return new KeyValuePair<string, string>(key, value);
                    }
                }
            }
        }

        public Dictionary<string, string> ParseAsDict(string[] args) {
            return Parse(args).ToDictionary(p => p.Key, p => p.Value);
        }

        public NameValueCollection ParseAsForm(string[] args) {
            NameValueCollection form = new NameValueCollection();
            foreach (var item in Parse(args)) {
                form.Set(item.Key, item.Value);
            }
            return form;
        }

        public string Combin(IEnumerable<KeyValuePair<string, string>> pairs) {
            if (pairs.Any()) {
                StringBuilder builder = new StringBuilder();
                Char start = Setting.GetStartChar();
                Char split = Setting.GetSplitChar();
                foreach (var pair in pairs) {
                    builder.Append(start);
                    Append(builder, pair.Key);
                    builder.Append(split);
                    Append(builder, pair.Value);
                    builder.Append(' ');
                }
                builder.Remove(builder.Length - 1, 1);
                return builder.ToString();
            }
            return string.Empty;
        }

        private static void Append(StringBuilder builder, string value) {
            Boolean needQuote = value.Contains(' ');
            if (needQuote) {
                builder.Append('"');
            }
            builder.Append(value);
            if (needQuote) {
                builder.Append('"');
            }
        }

        public string Combin(Object input) {
            var pairs = TypeDescriptor.GetProperties(input).OfType<PropertyDescriptor>()
                .Select(p => new KeyValuePair<string, string>(p.Name, p.GetValue(input).ToString()));
            return Combin(pairs);
        }
    }

}
