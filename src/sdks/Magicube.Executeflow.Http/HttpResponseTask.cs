using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Executeflow.Http {
    public class HttpResponseTask : Activity {
        private readonly EvaluatorFactory _evaluator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HttpResponseTask(
            IStringLocalizer localizer,
            EvaluatorFactory evaluator,
            IHttpContextAccessor httpContextAccessor
            ) : base(localizer) {
            _httpContextAccessor = httpContextAccessor;
            _evaluator = evaluator;
        }

        public override string Category => L["Http"];

        public ExecuteExpression<string> Content {
            get => GetProperty(() => new ExecuteExpression<string>(""));
            set => SetProperty(value);
        }

        public int HttpStatusCode {
            get => GetProperty(() => 200);
            set => SetProperty(value);
        }

        public ExecuteExpression<string> Headers {
            get => GetProperty(() => new ExecuteExpression<string>(""));
            set => SetProperty(value);
        }

        public ExecuteExpression<string> ContentType {
            get => GetProperty(() => new ExecuteExpression<string>("application/json"));
            set => SetProperty(value);
        }

        public override IEnumerable<Outcome> GetOutcomes(ExecuteflowContext workflowContext, CancellationToken cancellationToken = default) {
            return BuildOutcomes(OutcomeNames.Done);
        }

        protected override async Task<ActivityExecutionResult> OnExecute(ExecuteflowContext context, CancellationToken cancellationToken = default) {
            var (headersString ,_)= await _evaluator.Evaluate<string>(Headers, context);
            var (content       ,_)= await _evaluator.Evaluate<string>(Content, context);
            var (contentType   ,_)= await _evaluator.Evaluate<string>(ContentType, context);
            var headers = ParseHeaders(headersString);
            var response = _httpContextAccessor.HttpContext.Response;

            response.StatusCode = HttpStatusCode;

            foreach (var header in headers) {
                response.Headers.Add(header);
            }

            if (!string.IsNullOrWhiteSpace(contentType)) {
                response.ContentType = contentType;
            }

            if (!string.IsNullOrWhiteSpace(content)) {
                await response.WriteAsync(content, cancellationToken);
            }

            return Outcomes(OutcomeNames.Done);

        }

        private IEnumerable<KeyValuePair<string, StringValues>> ParseHeaders(string text) {
            if (string.IsNullOrWhiteSpace(text))
                return Enumerable.Empty<KeyValuePair<string, StringValues>>();

            return from header in text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                let pair = header.Split(':')
                where pair.Length == 2
                select new KeyValuePair<string, StringValues>(pair[0], pair[1]);
        }
    }
}
