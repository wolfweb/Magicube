using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Executeflow.Activities {
    public class SetProperty : Activity {
		private readonly EvaluatorFactory _evaluator;
		public SetProperty(EvaluatorFactory evaluator, IStringLocalizer localizer) : base(localizer) {
			_evaluator = evaluator;
		}

		public override string Category => L["Primitives"];

		public IExecuteflowExpression Value {
			get => GetProperty<IExecuteflowExpression>();
			set => SetProperty(value);
		}

		public override IEnumerable<Outcome> GetOutcomes(ExecuteflowContext context, CancellationToken cancellationToken = default) {
			return BuildOutcomes(OutcomeNames.Done);
		}

		protected override async Task<ActivityExecutionResult> OnExecute(ExecuteflowContext context, CancellationToken cancellationToken = default) {
			await _evaluator.Evaluate<object>(Value, context);
			return Outcomes(OutcomeNames.Done);
		}
	}
}
