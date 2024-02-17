using Magicube.Core.Environment.Variable;
using System;
using System.IO;

namespace Magicube.Core.EnvVariable {
    public class GetFileExtensionVariableHandler : BaseVariableHandler {
        public override string Name => "{FILE_EXT}";

        public override string Description => "获取source的文件扩展";

        public override string ParseVariable(string template, string source) {
            var res = template;
            var match = Find(res);
            if (match.Success) {
                res = template.Replace(match.Value, Path.GetExtension(source));
            }

            return res;
        }
    }
}
