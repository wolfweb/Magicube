using System.Collections.Generic;

namespace Magicube.Executeflow.Scripting {
    public class ExecuteflowHandlerContextBase {
        protected ExecuteflowHandlerContextBase(ExecuteflowContext context) {
            ExecuteflowContext = context;
        }

        public ExecuteflowContext ExecuteflowContext { get; }
    }

    public class ExecuteflowScriptContext : ExecuteflowHandlerContextBase {
        public ExecuteflowScriptContext(ExecuteflowContext context) : base(context) {
        }

        public IList<IGlobalMethodProvider> ScopedMethodProviders { get; } = new List<IGlobalMethodProvider>();
    }
}
