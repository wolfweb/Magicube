using Magicube.Core.Runtime;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Magicube.Core {
#if !DEBUG
using System.Diagnostics;
    [DebuggerStepThrough]
#endif
	public static class RuntimeMethodProviderExtension {
        public static Delegate ToDelegate(this RuntimeMethodProvider methodProvider, object instance) {
			Func<Type[], Type> getType;
			var isAction = methodProvider.Method.ReturnType.Equals(typeof(void));
			var types = methodProvider.Parameters.Select(x => x.ParameterType);

			if (isAction) {
				getType = Expression.GetActionType;
			} else {
				getType = Expression.GetFuncType;
				types = types.Concat(new[] { methodProvider.Method.ReturnType });
			}
			if (methodProvider.Method.IsStatic) {
				return Delegate.CreateDelegate(getType(types.ToArray()), methodProvider.Method);
			}
			return Delegate.CreateDelegate(getType(types.ToArray()), instance, methodProvider.Method.Name);
		}
    }
}
