using System;
using System.Collections.Generic;

namespace Magicube.Executeflow.Scripting {
    public interface IGlobalMethodProvider {
        IEnumerable<GlobalMethod> GetMethods();
    }

    public class ExecuteflowMethodsProvider : IGlobalMethodProvider {
        private readonly GlobalMethod _resultMethod;
        private readonly GlobalMethod _inputMethod;
        private readonly GlobalMethod _literalMethod;

        public ExecuteflowMethodsProvider(ExecuteflowContext ctx) {
            _inputMethod = new GlobalMethod {
                Name = "$",
                Method = serviceProvider => (Func<string, object>)((name) => ctx.Input.ContainsKey(name) ? ctx.Input[name] : null)
            };

            _resultMethod = new GlobalMethod {
                Name = "lastResult",
                Method = serviceProvider => (Func<object>)(() => ctx.LastResult)
            };

            _literalMethod = new GlobalMethod {
                Name = "literal",
                Method = serviceProvider => (Func<object, object>)(v => v)
            };
        }

        public IEnumerable<GlobalMethod> GetMethods() {
            return new[] { _inputMethod, _resultMethod, _literalMethod };
        }
    }
}
