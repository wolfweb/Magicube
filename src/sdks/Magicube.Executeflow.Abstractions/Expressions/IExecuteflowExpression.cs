using Magicube.Executeflow.Scripting;

namespace Magicube.Executeflow {
    public interface IExecuteflowExpression {
		string Keyed      { get; }
        string Expression { get; }

        T Execute<T>(ExecuteflowContext ctx, IScriptingScope scope);
    }

    public abstract class ExecuteflowExpression<T> : IExecuteflowExpression{
        public string   Keyed      { get; protected set; }
        public string   Expression { get; protected set; }

        public abstract T1 Execute<T1>(ExecuteflowContext ctx, IScriptingScope scope);
    }
}
