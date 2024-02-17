using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Executeflow.Activities {
    public class For : Activity, IActivityBlock {
		private readonly EvaluatorFactory _evaluator;
		public For(EvaluatorFactory evaluator, IStringLocalizer localizer) : base(localizer) {
			_evaluator = evaluator;
        }

		public override string Category => L["ControlFlow"];

		public override IEnumerable<Outcome> GetOutcomes(ExecuteflowContext context, CancellationToken cancellationToken = default) {
			return BuildOutcomes(OutcomeNames.Iterate, OutcomeNames.Done);
		}

		public IExecuteflowExpression From {
			get => GetProperty<IExecuteflowExpression>(() => new LiteralExpression<double>("From",0));
			set => SetProperty(value);
		}

		public IExecuteflowExpression To {
			get => GetProperty<IExecuteflowExpression>(() => new LiteralExpression<double>("To",10));
			set => SetProperty(value);
		}

		public IExecuteflowExpression Step {
			get => GetProperty<IExecuteflowExpression>(() => new LiteralExpression<double>("Step",1));
			set => SetProperty(value);
		}

		public double Index {
			get => GetProperty(() => 0);
			set => SetProperty(value);
		}

		public override Task OnActivityExecutingAsync(ExecuteflowContext context, ActivityContext activityContext, CancellationToken cancellationToken = default) {
			context.ContextScope.Push(activityContext.Activity, null);
			return Task.CompletedTask;
		}

		protected override async Task<ActivityExecutionResult> OnExecute(ExecuteflowContext context, CancellationToken cancellationToken = default) {
			var (from, _) = await _evaluator.Evaluate<double>(From, context);
			var (to, _)   = await _evaluator.Evaluate<double>(To, context);
			var (step, scope) = await _evaluator.Evaluate<double>(Step, context);

			if (Index < from) {
				Index = from;
			}

			if (Index < to) {
				context.LastResult = Index;
				Index += step;
				return Outcomes(OutcomeNames.Iterate);
			} else {
				Index = from;
				context.ContextScope.Done(context);
				return Outcomes(OutcomeNames.Done);
			}
		}
    }
}
