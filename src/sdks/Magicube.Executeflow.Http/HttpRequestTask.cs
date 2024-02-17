using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Executeflow.Http {
    public class HttpRequestTask : Activity {
        private static readonly Dictionary<int, string> HttpStatusCodeDictionary = new Dictionary<int, string>
        {
            { 100, "Continue" },
            { 101, " Switching Protocols" },
            { 102 , "Processing" },
            { 200 , "OK" },
            { 201 , "Created" },
            { 202 , "Accepted" },
            { 203 , "Non-authoritative Information" },
            { 204 , "No Content" },
            { 205 , "Reset Content" },
            { 206 , "Partial Content" },
            { 207 , "Multi-Status" },
            { 208 , "Already Reported" },
            { 226 , "IM Used" },
            { 300 , "Multiple Choices" },
            { 301 , "Moved Permanently" },
            { 302 , "Found" },
            { 303 , "See Other" },
            { 304 , "Not Modified" },
            { 305 , "Use Proxy" },
            { 307 , "Temporary Redirect" },
            { 308 , "Permanent Redirect" },
            { 400 , "Bad Request" },
            { 401 , "Unauthorized" },
            { 402 , "Payment Required" },
            { 403 , "Forbidden" },
            { 404 , "Not Found" },
            { 405 , "Method Not Allowed" },
            { 406 , "Not Acceptable" },
            { 407 , "Proxy Authentication Required" },
            { 408 , "Request Timeout" },
            { 409 , "Conflict" },
            { 410 , "Gone" },
            { 411 , "Length Required" },
            { 412 , "Precondition Failed" },
            { 413 , "Payload Too Large" },
            { 414 , "Request-URI Too Long" },
            { 415 , "Unsupported Media Type" },
            { 416 , "Requested Range Not Satisfiable" },
            { 417 , "Expectation Failed" },
            { 418 , "I'm a teapot" },
            { 421 , "Misdirected Request" },
            { 422 , "Unprocessable Entity" },
            { 423 , "Locked" },
            { 424 , "Failed Dependency" },
            { 426 , "Upgrade Required" },
            { 428 , "Precondition Required" },
            { 429 , "Too Many Requests" },
            { 431 , "Request Header Fields Too Large" },
            { 444 , "Connection Closed Without Response" },
            { 451 , "Unavailable For Legal Reasons" },
            { 499 , "Client Closed Request" },
            { 500 , "Internal Server Error" },
            { 501 , "Not Implemented" },
            { 502 , "Bad Gateway" },
            { 503 , "Service Unavailable" },
            { 504 , "Gateway Timeout" },
            { 505 , "HTTP Version Not Supported" },
            { 506 , "Variant Also Negotiates" },
            { 507 , "Insufficient Storage" },
            { 508 , "Loop Detected" },
            { 510 , "Not Extended" },
            { 511 , "Network Authentication Required" },
            { 599 , "Network Connect Timeout Error" }
        };
        private static HttpClient _httpClient;

        private readonly EvaluatorFactory _evaluator;
        public HttpRequestTask(EvaluatorFactory evaluator, IHttpClientFactory httpClient, IStringLocalizer localizer) : base(localizer) {
            _evaluator = evaluator;
            if(_httpClient == null) _httpClient = httpClient.CreateClient();
        }

        public override string Category => "Http";

        public string Url {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public string HttpMethod {
            get => GetProperty(() => "Get");
            set => SetProperty(value);
        }

        public IExecuteflowExpression Headers {
            get => GetProperty<IExecuteflowExpression>();
            set => SetProperty(value);
        }

        public IExecuteflowExpression Body {
            get => GetProperty<IExecuteflowExpression>();
            set => SetProperty(value);
        }

        public string ContentType {
            get => GetProperty(() => "application/json");
            set => SetProperty(value);
        }

        public string HttpResponseCodes {
            get => GetProperty(() => "200");
            set => SetProperty(value);
        }

        public override IEnumerable<Outcome> GetOutcomes(ExecuteflowContext context, CancellationToken cancellationToken = default) {
            var outcomes = !string.IsNullOrWhiteSpace(HttpResponseCodes)
                ? HttpResponseCodes.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => {
                    var status = int.Parse(x.Trim());
                    var description = HttpStatusCodeDictionary.TryGetValue(status, out var text) ? $"{status} {text}" : status.ToString();
                    return new Outcome(status.ToString());
                }).ToList()
                : new List<Outcome>();

            outcomes.Add(new Outcome("UnhandledHttpStatus"));
            return outcomes;
        }

        protected override async Task<ActivityExecutionResult> OnExecute(ExecuteflowContext context, CancellationToken cancellationToken = default) {
            var (headersText,_) = await _evaluator.Evaluate<string>(Headers, context);
            var headers     = ParseHeaders(headersText);

            var httpMethod = HttpMethod;
            var request    = new HttpRequestMessage(new HttpMethod(httpMethod), Url);
            foreach (var header in headers) {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            if (HttpMethods.IsPatch(httpMethod) || HttpMethods.IsPost(httpMethod) || HttpMethods.IsPut(httpMethod)) {
                var (body,_)    = await _evaluator.Evaluate<string>(Body, context);
                request.Content = new StringContent(body, Encoding.UTF8, ContentType);
            }

            var response      = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
            var responseCodes = ParseResponseCodes(HttpResponseCodes);
            var outcome       = responseCodes.FirstOrDefault(x => x == (int)response.StatusCode);

            context.LastResult = new {
                Body                = await response.Content.ReadAsStringAsync(),
                Headers             = response.Headers.ToDictionary(x => x.Key),
                StatusCode          = response.StatusCode,
                ReasonPhrase        = response.ReasonPhrase,
                IsSuccessStatusCode = response.IsSuccessStatusCode
            };

            return Outcomes(outcome != 0 ? outcome.ToString() : "UnhandledHttpStatus");
        }

        private IEnumerable<KeyValuePair<string, string>> ParseHeaders(string text) {
            if (string.IsNullOrWhiteSpace(text))
                return Enumerable.Empty<KeyValuePair<string, string>>();

            return from header in text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                let pair = header.Split(':', 2)
                where pair.Length == 2
                select new KeyValuePair<string, string>(pair[0], pair[1]);
        }

        private IEnumerable<int> ParseResponseCodes(string text) {
            return from code in text.Split(',', StringSplitOptions.RemoveEmptyEntries)
                select int.Parse(code);
        }
    }
}
