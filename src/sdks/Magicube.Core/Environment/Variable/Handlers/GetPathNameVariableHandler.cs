using Magicube.Core.Environment.Variable;
using System;
using System.IO;

namespace Magicube.Core.EnvVariable {
    public class GetPathNameVariableHandler : BaseVariableHandler {
        public override string Name => "{PATH_NAME}";

        public override int Priority => 100;

        public override string Description => "获取source的路径名";

        public override string ParseVariable(string template, string source) {
            var res = template;
            var match = Find(res);
            if (match.Success) {
                res = template.Replace(match.Value, Path.GetFileName(Path.GetDirectoryName(source)));
            }
            return res;
        }
    }
}
