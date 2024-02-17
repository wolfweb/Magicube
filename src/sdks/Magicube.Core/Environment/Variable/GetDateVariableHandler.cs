using System;
using System.Text.RegularExpressions;

namespace Magicube.Core.Environment.Variable {
    public class GetDateVariableHandler : BaseVariableHandler {
        public override string Name => "{DATE}";

        public override string Description => "格式化日期，格式{DATE[:yyyyMMdd]},支持自定义格式化";

        public override string ParseVariable(string template, string source) {
            var res = template;
            var match = Regex.Match(template, @"{DATE\:?(?<v>[^\}]*)}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (match.Success) {
                var format = match.Groups["v"].Value.IsNullOrEmpty() ? "yyyyMMdd" : match.Groups["v"].Value;
                res = template.Replace(match.Value, DateTime.Now.ToString(format));
            }
            return res;
        }
    }
}
