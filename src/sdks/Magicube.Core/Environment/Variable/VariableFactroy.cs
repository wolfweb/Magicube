using System.Collections.Generic;
using System.Linq;

namespace Magicube.Core.Environment.Variable {
    public class VariableFactroy {
        private readonly IEnumerable<IVariableHandler> _envVariableHandlers;
        public VariableFactroy(IEnumerable<IVariableHandler> envVariableHandlers) {
            _envVariableHandlers = envVariableHandlers.OrderBy(x=>x.Priority);
        }

        public string Parse(string template, string source) {
            var res = template;
            foreach (var handler in _envVariableHandlers) {
                res = handler.ParseVariable(res, source);
            }
            return res;
        }
    }
}
