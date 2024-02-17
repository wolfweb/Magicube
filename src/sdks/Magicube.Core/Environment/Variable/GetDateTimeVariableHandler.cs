using System;
using System.Text.RegularExpressions;

namespace Magicube.Core.Environment.Variable {
    public class GetDateTimeVariableHandler : BaseVariableHandler {
        public override string Name => "{NOW}";

        public override string Description => "格式化日期时间，格式{NOW[:yyyyMMddHHmmss]},支持自定义格式化";

        public override string ParseVariable(string template, string source) {
            var res = template;
            var match = Regex.Match(template, @"{NOW\:?(?<v>[^\}]*)}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (match.Success) {
                var format = match.Groups["v"].Value.IsNullOrEmpty() ? "yyyyMMddHHmmss" : match.Groups["v"].Value;
                res = template.Replace(match.Value, DateTime.Now.ToString(format));
            }
            return res;
        }
    }
}
