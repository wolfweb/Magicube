using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Executeflow.Activities {
    public class IfElse : Activity, IActivityBlock {
		private readonly EvaluatorFactory _evaluator;
		public IfElse(EvaluatorFactory evaluator, IStringLocalizer localizer) : base(localizer) {
			_evaluator = evaluator;
		}

		public override string Category => L["ControlFlow"];

		public IExecuteflowExpression Condition {
			get => GetProperty<IExecuteflowExpression>(() => new ExecuteExpression<bool>("true"));
			set => SetProperty(value);
		}

		public override IEnumerable<Outcome> GetOutcomes(ExecuteflowContext context, CancellationToken cancellationToken = default) {
			return BuildOutcomes(OutcomeNames.True, OutcomeNames.False);
		}

		public override Task OnActivityExecutingAsync(ExecuteflowContext context, ActivityContext activityContext, CancellationToken cancellationToken = default) {
			context.ContextScope.Push(activityContext.Activity, null);
			return Task.CompletedTask;
		}

		protected override async Task<ActivityExecutionResult> OnExecute(ExecuteflowContext context, CancellationToken cancellationToken = default) {
			var (res,_) = await _evaluator.Evaluate<bool>(Condition, context);
			return Outcomes(res ? OutcomeNames.True : OutcomeNames.False);
		}
    }
}
