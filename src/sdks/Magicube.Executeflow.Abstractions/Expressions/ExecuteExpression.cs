using Magicube.Core;
using Magicube.Core.Models;
using Magicube.Executeflow.Scripting;

namespace Magicube.Executeflow {
    public class ExecuteExpression<T> : ExecuteflowExpression<T> {
		public const string Key = "execute";
		public ExecuteExpression(string expression) {
			Keyed      = Key;
			Expression = expression;
		}

		public override T1 Execute<T1>(ExecuteflowContext ctx, IScriptingScope scope) {
			if (!Expression.IsNullOrEmpty()) {
				var Value = scope.Execute(this);
				return Value.GetType().IsSimpleType() ? new ValueObject(Value).ConvertTo<T1>() : (T1)Value;
			}
			return default;
		}
	}

    public class ExecuteExpression : ExecuteExpression<string> {
        public ExecuteExpression(string expression) : base(expression) { }

        public static implicit operator ExecuteExpression(string expression) {
            return new ExecuteExpression(expression);
        }
    }
}
