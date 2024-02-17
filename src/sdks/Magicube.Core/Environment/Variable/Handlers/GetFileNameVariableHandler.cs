using Magicube.Core.Environment.Variable;
using System;
using System.IO;

namespace Magicube.Core.EnvVariable {
    public class GetFileNameVariableHandler : BaseVariableHandler {
        public override string Name => "{FILE_NAME}";

        public override int Priority => 100;

        public override string Description => "获取source的文件名";

        public override string ParseVariable(string template, string source) {
            var res = template;
            var match = Find(res);
            if (match.Success) {
                res = template.Replace(match.Value, Path.GetFileName(source));
            }

            return res;
        }
    }
}
