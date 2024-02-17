using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Magicube.Web.UI.TagHelpers {
    public static class RenderUtils {
        public static string HighlightSearchString(this string text, string keywords, bool fullMatch = false, string cssClass = "highlight") {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(keywords))
                return text;

            var words = keywords.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (!fullMatch)
                return words.Select(word => word.Trim()).Aggregate(text,
                   (current, pattern) =>
                      Regex.Replace(current,
                         pattern,
                          $"<span class=\"{cssClass}\">$0</span>",
                         RegexOptions.IgnoreCase));

            return words.Select(word => "\\b" + word.Trim() + "\\b")
               .Aggregate(text, (current, pattern) =>
                  Regex.Replace(current,
                     pattern,
                      $"<span class=\"{cssClass}\">$0</span>",
                     RegexOptions.IgnoreCase));

        }

        public static List<string> GetCustomLinkParamters(string customLink) {
            var item = "";
            var start = false;
            var ret = new List<string>();

            foreach (var c in customLink) {
                if (c == '{') {
                    start = true;
                    continue;
                }
                if (c == '}') {
                    start = false;
                    ret.Add(item);
                    item = "";
                    continue;
                }

                if (start)
                    item += c;
            }

            return ret;
        }
    }
}