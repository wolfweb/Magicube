using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Executeflow {
	public interface IActivityExecutionResult {
		ValueTask ExecuteAsync(ExecuteflowContext executionContext, CancellationToken cancellationToken = default);
	}

    public abstract class ActivityExecutionResult : IActivityExecutionResult {
        public virtual ValueTask ExecuteAsync(ExecuteflowContext executionContext, CancellationToken cancellationToken = default) {
			return new ValueTask();
        }
    }

    public class OutcomeExecutionResult : ActivityExecutionResult {
		public IEnumerable<string> Outcomes { get; private set; }

		public OutcomeExecutionResult(IEnumerable<string> outcomes) {
			Outcomes = outcomes;
		}

        public override ValueTask ExecuteAsync(ExecuteflowContext executionContext, CancellationToken cancellationToken) {
            var conn = executionContext.Execflow.Connections.FirstOrDefault(x => x.SourceActivityId == executionContext.CurrentActivityContext.Entity.Id && Outcomes.Contains(x.SourceOutcomeName));
            if (conn != null) {
                executionContext.ScheduleActivity(conn);
            }
            return base.ExecuteAsync(executionContext, cancellationToken);
        }
    }

    public class NoopExecutionResult : ActivityExecutionResult { }

    public class DoneExecutionResult : OutcomeExecutionResult {
        public DoneExecutionResult() : base(new[] { OutcomeNames.Done }) {
        }
    }

    public class FaultExecutionResult : ActivityExecutionResult {
        public Exception Exception { get; }
        public FaultExecutionResult(Exception ex) {
            Exception = ex;
        }
        public override ValueTask ExecuteAsync(ExecuteflowContext executionContext, CancellationToken cancellationToken) {
            executionContext.Fault(Exception);
            return base.ExecuteAsync(executionContext, cancellationToken);
        }
    }

    public class SuspendExecutionResult : IActivityExecutionResult {
        public ValueTask ExecuteAsync(ExecuteflowContext executionContext, CancellationToken cancellationToken = default) {
            executionContext.Suspend();
            return new ValueTask();
        }
    }
}
