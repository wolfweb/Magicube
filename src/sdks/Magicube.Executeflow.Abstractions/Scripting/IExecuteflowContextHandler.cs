using System.Threading.Tasks;

namespace Magicube.Executeflow.Scripting {
    public interface IExecuteflowContextHandler {
        public virtual Task EvaluatingScriptAsync(ExecuteflowScriptContext context) {
            return Task.CompletedTask;
        }
    }

    public class DefaultExecuteflowContextHandler : IExecuteflowContextHandler {
        Task IExecuteflowContextHandler.EvaluatingScriptAsync(ExecuteflowScriptContext context) {
            context.ScopedMethodProviders.Add(new ExecuteflowMethodsProvider(context.ExecuteflowContext));
            return Task.CompletedTask;
        }
    }
}
