using Magicube.Core;
using Magicube.Core.Models;
using Magicube.Core.Reflection;
using Magicube.Executeflow.Scripting;

namespace Magicube.Executeflow {
    public interface IVariableExpression {
        string Name  { get; }
        object Value { get; set; }
    }

    public class VariableExpression<T> : ExecuteflowExpression<T>, IVariableExpression {
        public const string Key = "variable";
        public VariableExpression(string name, object value) {
            Value = value;
            Name  = name;
            Keyed = Key;
        }

        public string Name  { get; protected set; }
        public object Value { get; set; }

        public override T1 Execute<T1>(ExecuteflowContext ctx, IScriptingScope scope) {
            if (Value is  IExecuteflowExpression expression) {
                scope.LoadVariable(Name, expression.Execute<object>(ctx, scope));
            } else {
                scope.LoadVariable(Name, Value);
            }
            ctx?.ContextScope.Push(ctx.CurrentActivityContext.Activity, this);
            return (T1)scope.GetVariable(Name);
        }
    }

    public class VariableExpression : VariableExpression<object> {
        public VariableExpression(string name, object value) : base(name, value) { }
    }
}
