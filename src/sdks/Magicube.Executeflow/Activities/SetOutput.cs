using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Executeflow.Activities {
    public class SetOutput : Activity {
		private readonly EvaluatorFactory _evaluator;
		public SetOutput(EvaluatorFactory evaluator, IStringLocalizer localizer) : base(localizer) {
			_evaluator = evaluator;
		}
		public override string Category => L["Primitives"];

		public string OutputName {
			get => GetProperty<string>();
			set => SetProperty(value);
		}

		public IExecuteflowExpression Value {
			get => GetProperty<IExecuteflowExpression>();
			set => SetProperty(value);
		}

		public override IEnumerable<Outcome> GetOutcomes(ExecuteflowContext context, CancellationToken cancellationToken = default) {
			return BuildOutcomes(L[OutcomeNames.Done]);
		}

		protected override async Task<ActivityExecutionResult> OnExecute(ExecuteflowContext context, CancellationToken cancellationToken = default) {
			await _evaluator.Evaluate<object>(Value, context);
			return Outcomes(OutcomeNames.Done);
		}
	}
}
