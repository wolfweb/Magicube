using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Executeflow.Http {
    public class HttpRedirectTask : Activity {
        private readonly EvaluatorFactory _evaluator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpRedirectTask(
            IStringLocalizer localizer,
            EvaluatorFactory evaluator,
            IHttpContextAccessor httpContextAccessor
            ) : base(localizer) {
            _httpContextAccessor = httpContextAccessor;
            _evaluator = evaluator;
        }

        public override string Category => L["HTTP"];

        public override IEnumerable<Outcome> GetOutcomes(ExecuteflowContext context, CancellationToken cancellationToken = default) {
            return BuildOutcomes(OutcomeNames.Done);
        }

        public ExecuteExpression<string> Location {
            get => GetProperty(() => new ExecuteExpression<string>(""));
            set => SetProperty(value);
        }

        public bool Permanent {
            get => GetProperty(() => false);
            set => SetProperty(value);
        }

        protected override async Task<ActivityExecutionResult> OnExecute(ExecuteflowContext context, CancellationToken cancellationToken = default) {
            var (location, _) = await _evaluator.Evaluate<string>(Location, context);
            _httpContextAccessor.HttpContext.Response.Redirect(location, Permanent);
            return Outcomes(OutcomeNames.Done);
        }
    }
}
