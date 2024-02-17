using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Executeflow.Activities {
    public class SetInput : Activity {
        private readonly EvaluatorFactory _evaluator;
        public SetInput(IStringLocalizer localizer, EvaluatorFactory evaluator) : base(localizer) {
            _evaluator = evaluator;
        }

        public override string Category => L["Primitives"];

        public string VariableName {
            get;
            set;
        } = "x";


        public IExecuteflowExpression Variable {
			get => GetProperty<IExecuteflowExpression>();
			set => SetProperty(value);
        }

        public override IEnumerable<Outcome> GetOutcomes(ExecuteflowContext context, CancellationToken cancellationToken = default) {
			return BuildOutcomes(OutcomeNames.Next);
        }

        protected override async Task<ActivityExecutionResult> OnExecute(ExecuteflowContext context, CancellationToken cancellationToken = default) {
            await _evaluator.Evaluate<object>(Variable, context);
            return Outcomes(OutcomeNames.Next);
        }
    }
}
