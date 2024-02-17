using Magicube.Core;
using Magicube.Core.Models;
using Magicube.Executeflow.Scripting;

namespace Magicube.Executeflow {
	public interface ILiteralExpression {
		string Name  { get; }
		object Value { get; }
	}

	public class LiteralExpression<T> : ExecuteflowExpression<T> {
		public const string Key = "literal";
		public LiteralExpression(string name, object value) {
			Value = value;
			Name  = name;
			Keyed = Key;
		}

		public string Name  { get; }
		public object Value { get; }

		public override T1 Execute<T1>(ExecuteflowContext ctx, IScriptingScope scope) {
			if (Value is IExecuteflowExpression expression) {
				scope.LoadVariable(Name, expression.Execute<object>(ctx, scope));
			} else {
				scope.LoadVariable(Name, Value);
			}
			return Value.GetType().IsSimpleType() ? new ValueObject(Value).ConvertTo<T1>() : (T1)Value;
		}
	}

	public class LiteralExpression : LiteralExpression<object> {
		public LiteralExpression(string name, object value) : base(name, value) { }
	}
}
