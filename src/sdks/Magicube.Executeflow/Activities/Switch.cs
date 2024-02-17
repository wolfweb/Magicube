using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Executeflow.Activities {
    public class Switch : Activity {
		private readonly EvaluatorFactory _evaluator;
        public Switch(
			IStringLocalizer localizer, 
			EvaluatorFactory evaluator
			) : base(localizer) {
            _evaluator = evaluator;
        }

        public override string Category => L["ControlFlow"];

		public IExecuteflowExpression Cases {
			get => GetProperty<IExecuteflowExpression>(() => new ExecuteExpression<List<string>>("[]"));
			set => SetProperty(value);
		}

		public IExecuteflowExpression Condition {
			get => GetProperty<IExecuteflowExpression>(() => new ExecuteExpression<bool>("true"));
			set => SetProperty(value);
		}

		public string CaseItemName => "x";

		public override IEnumerable<Outcome> GetOutcomes(ExecuteflowContext context, CancellationToken cancellationToken = default) {
			return BuildOutcomes(OutcomeNames.Done);
        }

        protected override async Task<ActivityExecutionResult> OnExecute(ExecuteflowContext context, CancellationToken cancellationToken = default) {
			var (cases, _) = await _evaluator.Evaluate<string[]>(Cases, context);
			foreach (var item in cases) {
				await _evaluator.Evaluate<bool>(Condition, context);
			}

			return Outcomes(OutcomeNames.Done);
        }
    }
}
