using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Executeflow.Activities {
    public class ForEach : Activity, IActivityBlock {
		private readonly EvaluatorFactory _evaluator;
		public ForEach(EvaluatorFactory evaluator, IStringLocalizer localizer) : base(localizer) {
			_evaluator = evaluator;
		}
		public override string Category => L["ControlFlow"];

		public IExecuteflowExpression Collection {
			get => GetProperty(() => new LiteralExpression<List<object>>("Collection",Enumerable.Empty<object>()));
			set => SetProperty(value);
		}
		
		public object Current {
			get => GetProperty<object>();
			set => SetProperty(value);
		}

		public int Index {
			get => GetProperty(() => 0);
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
			var (items, scope) = await _evaluator.Evaluate<List<object>>(Collection, context);
            var count = items.Count;

            if (Index < count) {
                var current = Current = items[Index];
                context.LastResult = current;
                Index++;
                return Outcomes(OutcomeNames.Iterate);
            } else {
                Index = 0;
				context.ContextScope.Done(context);
				return Outcomes(OutcomeNames.Done);
            }
        }
    }
}
