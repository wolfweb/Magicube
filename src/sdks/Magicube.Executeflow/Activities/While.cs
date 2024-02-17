using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Executeflow.Activities {
    public class While : Activity, IActivityBlock {
		private readonly EvaluatorFactory _evaluator;

		public While(EvaluatorFactory evaluator, IStringLocalizer localizer) : base(localizer) {
			_evaluator = evaluator;
		}

		public override string Category => L["ControlFlow"];

		public IExecuteflowExpression Condition {
			get => GetProperty<IExecuteflowExpression>();
			set => SetProperty(value);
		}

		public override IEnumerable<Outcome> GetOutcomes(ExecuteflowContext context, CancellationToken cancellationToken = default) {
			return BuildOutcomes(OutcomeNames.Iterate, OutcomeNames.Done);
		}

        public override Task OnActivityExecutingAsync(ExecuteflowContext context, ActivityContext activityContext, CancellationToken cancellationToken = default) {
			context.ContextScope.Push(activityContext.Activity, null);

			return Task.CompletedTask;
        }

        protected override async Task<ActivityExecutionResult> OnExecute(ExecuteflowContext context, CancellationToken cancellationToken = default) {
			var (res,scope) = await _evaluator.Evaluate<bool>(Condition, context);
			if (res) return Outcomes(OutcomeNames.Iterate);
			
			context.ContextScope.Done(context);
			return Outcomes(OutcomeNames.Done);
		}
	}
}
