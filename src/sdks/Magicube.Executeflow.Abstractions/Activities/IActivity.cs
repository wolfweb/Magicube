using Magicube.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Executeflow {
    public interface IActivity {
		string                         Id         { get; }
		string                         Name       { get; }
			                           		   
		string                         Category   { get; }
						               
		TransferContext                Properties { get; set; }
							           
		IEnumerable<Outcome>           GetOutcomes(ExecuteflowContext context, CancellationToken cancellationToken = default);
							           
		Task<bool>                     CanExecuteAsync(ExecuteflowContext context, CancellationToken cancellationToken = default);

		Task<ActivityExecutionResult>  ExecuteAsync(ExecuteflowContext context, CancellationToken cancellationToken = default);

		Task<ActivityExecutionResult>  ResumeAsync(ExecuteflowContext context, CancellationToken cancellationToken = default);

		Task                           OnActivityExecutingAsync(ExecuteflowContext context, ActivityContext activityContext, CancellationToken cancellationToken = default(CancellationToken));
			                           
		Task                           OnActivityExecutedAsync(ExecuteflowContext context, ActivityContext activityContext);
	}
}
