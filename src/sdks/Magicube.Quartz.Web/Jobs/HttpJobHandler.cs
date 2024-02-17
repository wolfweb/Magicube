using Magicube.Net;
using Magicube.Quartz.Jobs;
using Magicube.Quartz.Web.Models;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Magicube.Quartz.Web.Jobs {
    public class HttpJobHandler : JobBaseHandler {
        private readonly Curl _curl;

        public HttpJobHandler(
            ILogger logger, 
            IStringLocalizer<HttpJobHandler> localizer, 
            IOptions<JobOptions> options, 
            Curl curl) : base(logger, localizer, options) {
            _curl = curl;
        }

        public override string JobType => L["http请求"];

        public override async Task ExecuteAsync() {
            var model = DataContext.TryGet<HttpRequestModel>("Model");
            if (model == null) {
                Logger.LogWarning("http job requst http request model");
                return;
            }
            if (!Uri.IsWellFormedUriString(model.Url, UriKind.RelativeOrAbsolute)) {
                Logger.LogWarning($"invalid http request url {model.Url}");
                return;
            }

            _curl.Initialize(client => {
                if (model.Headers != null && model.Headers.Any()) {
                    foreach (var it in model.Headers) {
                        client.DefaultRequestHeaders.Add(it.Key, it.Value);
                    }
                }
            });

            string result = string.Empty;

            switch (model.Method.Method.ToUpper()) {
                case "GET":
                    result = await _curl.Get(model.Url).ReadAsString();
                    break;
                case "HEAD":
                    _curl.Head(model.Url);
                    break;
                case "POST":
                    result = await _curl.Post(model.Url, model.ContentType, model.Parameters).ReadAsString();
                    break;
                case "PUT":
                    result = await _curl.Put(model.Url, model.ContentType, model.Parameters).ReadAsString();
                    break;
                case "DELETE":
                    result = await _curl.Delete(model.Url, model.ContentType, model.Parameters).ReadAsString();
                    break;
            }

            Logger.LogDebug($"http request {model.Url} , {model.Method} \n {result}");
        }
    }
}
