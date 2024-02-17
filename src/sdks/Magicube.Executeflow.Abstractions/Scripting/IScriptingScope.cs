using System.Collections.Generic;

namespace Magicube.Executeflow.Scripting {
    public interface IScriptingScope {
        object GetVariable(string name);
        void   LoadVariable(string name, object value);
        void   LoadVariables(IDictionary<string, object> variables);
        object Execute(IExecuteflowExpression expression);
    }
}
