using Magicube.Executeflow.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magicube.Executeflow.Services {
	public interface IExecuteflowService {
		Task<ExecuteflowContext> CreateExecutionContextAsync(ExecuteflowStore execflow, IDictionary<string, object> input = null);
		Task Start(ExecuteflowContext execflowExecutionCtx);
	}

    public class ExecuteflowService : IExecuteflowService {
		private readonly IActivityProvider _activityProvider;
		public ExecuteflowService(IActivityProvider activityProvider) {
			_activityProvider = activityProvider;
		}

		public async Task<ExecuteflowContext> CreateExecutionContextAsync(ExecuteflowStore execflow, IDictionary<string, object> input = null) {
			return await Task.FromResult(new ExecuteflowContext(input, execflow, _activityProvider));
		}

		public async Task Start(ExecuteflowContext execflowExecutionCtx) {
			while (true) {
				try {
					var activityCtx = execflowExecutionCtx.Pop();
					if (activityCtx == null) break;
					
					await activityCtx.Activity.OnActivityExecutingAsync(execflowExecutionCtx, activityCtx);
					var result = await activityCtx.Activity.ExecuteAsync(execflowExecutionCtx);
					await activityCtx.Activity.OnActivityExecutedAsync(execflowExecutionCtx, activityCtx);

					await result.ExecuteAsync(execflowExecutionCtx);
				} catch (Exception e) {
					execflowExecutionCtx.Fault(e);
					break;
				}
			}
		}
	}
}
