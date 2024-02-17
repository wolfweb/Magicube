using Magicube.Core;
using Magicube.Executeflow.Scripting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magicube.Executeflow {
    public class EvaluatorFactory {
        private readonly ILogger _logger;
		private readonly IEnumerable<IScriptingEngine> _scriptingEngines;
        private readonly IEnumerable<IExecuteflowContextHandler> _executeflowContextHandlers;
        public EvaluatorFactory(
            ILogger<EvaluatorFactory> logger,
            IEnumerable<IScriptingEngine> scriptingEngines, 
            IEnumerable<IExecuteflowContextHandler> executeflowContextHandlers
            ) {
            _logger                     = logger;
            _scriptingEngines           = scriptingEngines;
            _executeflowContextHandlers = executeflowContextHandlers;
        }

        public async Task<(T,IScriptingScope)> Evaluate<T>(IExecuteflowExpression expression, ExecuteflowContext context, Action<IScriptingScope> handler = null) {
            var expressionContext = new ExecuteflowScriptContext(context);

            await _executeflowContextHandlers.InvokeAsync((h, expressionContext) => h.EvaluatingScriptAsync(expressionContext), expressionContext, _logger);

            var engines = _scriptingEngines.FirstOrDefault(x => x.CanExecute(expression.Keyed));
            var scope = engines.CreateScope(expressionContext.ScopedMethodProviders.SelectMany(x => x.GetMethods()));
            context.ContextScope.LoadAllVariables(scope);
            handler?.Invoke(scope);
            var result = await Task.FromResult(expression.Execute<T>(context, scope));
            context.ContextScope.Refresh(scope);
            context.LastResult = result;
            return (result, scope);
        }

        public Task<(T,IScriptingScope)> Evaluate<T>(string expression, ExecuteflowContext executionContext) {
            ExecuteExpression scriptExpression = expression;            
            return Evaluate<T>(scriptExpression, executionContext, null);
        }
    }
}
