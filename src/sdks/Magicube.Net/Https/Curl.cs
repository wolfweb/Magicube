using Magicube.Core.Convertion;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Magicube.Core;
using System.Threading.Tasks;
using System.Threading;

namespace Magicube.Net {
    public class Curl {
        public bool    IgnoreError     { get; set; }
        public string  UserIdentity    { get; set; }
        public int     Timeout         { get; set; } = 3000;

        public const string JSON      = "application/json";
        public const string FORM      = "application/x-www-form-urlencoded";
        public const string EMPTY_MD5 = "D41D8CD98F00B204E9800998ECF8427E";

        private readonly IHttpClientFactory _httpClientFactory;

        private Action<HttpClient> _initHandler;

        public ILogger<Curl> Logger { get; }

        public Curl(ILogger<Curl> logger, IHttpClientFactory httpClientFactory) {
            Logger             = logger;
            _httpClientFactory = httpClientFactory;
        }

        public void Initialize(Action<HttpClient> handler) {
            _initHandler = handler;
        }

        [DebuggerStepThrough]
        public CurlContext Get(string url, Action<HttpRequestMessage> callback = null) {
            return Invoke("GET", url, null, null, callback);
        }

        [DebuggerStepThrough]
        public CurlContext Head(string url, Action<HttpRequestMessage> callback = null) {
            return Invoke("HEAD", url, null, null, callback);
        }

        [DebuggerStepThrough]
        public CurlContext Post(string url, string mediaType, string data, Action<HttpRequestMessage> callback = null) {
            return Invoke("POST", url, mediaType, data, callback);
        }

        [DebuggerStepThrough]
        public CurlContext Post(string url, FileParameter data, Action<HttpRequestMessage> callback = null) {
            return Invoke("POST", url, null, data, callback);
        }

        [DebuggerStepThrough]
        public CurlContext Post(string url, Stream data, Action<HttpRequestMessage> callback = null) {
            return Invoke("POST", url, null, data, callback);
        }

        [DebuggerStepThrough]
        public CurlContext Put(string url, string mediaType, string data, Action<HttpRequestMessage> callback = null) {
            return Invoke("PUT", url, mediaType, data, callback);
        }

        [DebuggerStepThrough]
        public CurlContext Put(string url, FileParameter data, Action<HttpRequestMessage> callback = null) {
            return Invoke("PUT", url, null, data, callback);
        }

        [DebuggerStepThrough]
        public CurlContext Put(string url, Stream data, Action<HttpRequestMessage> callback = null) {
            return Invoke("PUT", url, null, data, callback);
        }

        [DebuggerStepThrough]
        public CurlContext PostJson(string url, string data, Action<HttpRequestMessage> callback = null) {
            return Invoke("POST", url, JSON, data, callback);
        }

        [DebuggerStepThrough]
        public CurlContext PostJson(string url, Object data, Action<HttpRequestMessage> callback = null) {
            return Invoke("POST", url, JSON, Json.Stringify(data), callback);
        }

        [DebuggerStepThrough]
        public CurlContext Delete(string url, Action<HttpRequestMessage> callback = null) {
            return Invoke("Delete", url, null, null, callback);
        }

        [DebuggerStepThrough]
        public CurlContext Delete(string url, string mediaType, string data, Action<HttpRequestMessage> callback = null) {
            return Invoke("Delete", url, mediaType, data, callback);
        }

        [DebuggerStepThrough]
        public CurlContext Upload(string url, IDictionary<string, object> postParameters, string method = "POST", Action<HttpRequestMessage> callback = null) {
            var httpClient = _httpClientFactory.CreateClient(ServiceCollectionExtension.HttpClientName);
            EnsureInitialize(httpClient);
            long num = DateTime.Now.Ticks;
            string text = "----" + num.ToString("x");
            using (var multiContent = new MultipartFormDataContent(text)) {
                foreach (var item in postParameters) {
                    if (item.Value is FileParameter file) {
                        var ctx = new StreamContent(file.Stream);
                        ctx.Headers.ContentDisposition = ContentDispositionHeaderValue.Parse($"form-data; name=\"{file.FileName ?? item.Key}\"; filename=\"{file.FileName ?? item.Key}\"");
                        ctx.Headers.ContentType        = file.ContentType.IsNullOrEmpty() ?  MediaTypeHeaderValue.Parse("application/octet-stream") : MediaTypeHeaderValue.Parse(file.ContentType);
                        multiContent.Add(ctx, file.FileName ?? item.Key, file.FileName ?? item.Key);
                    } else {
                        multiContent.Add(new StringContent(item.Value.ToString()), item.Key);
                    }
                }

                multiContent.Headers.ContentType = MediaTypeHeaderValue.Parse($"multipart/form-data; boundary={multiContent.Headers.ContentType.Parameters.First().Value.Replace("\"", "")}");
                var request = new HttpRequestMessage(new HttpMethod(method), url) {
                    Content = multiContent
                };
                callback?.Invoke(request);
                HttpResponseMessage response = httpClient.Send(request);
                return SendRequest(null, response);
            }
        }

        [DebuggerStepThrough]
        public CurlContext Send(HttpRequestMessage request) {
            var httpClient = _httpClientFactory.CreateClient(ServiceCollectionExtension.HttpClientName);
            EnsureInitialize(httpClient);
            var response = httpClient.Send(request);
            return SendRequest(null, response);
        }

        [DebuggerStepThrough]
        public async Task<CurlContext> SendAsync(HttpRequestMessage request) {
            var httpClient = _httpClientFactory.CreateClient(ServiceCollectionExtension.HttpClientName);
            EnsureInitialize(httpClient);
            var response = await httpClient.SendAsync(request);
            return SendRequest(request.Content.ReadAsStream(), response);
        }

        [DebuggerStepThrough]
        public CurlContext Invoke(string method, string url, string mediaType, object data, Action<HttpRequestMessage> callback = null) {
            return SendRequest(new HttpMethod(method), url, mediaType, data, callback);
        }

        private HttpResponseMessage CreateRequest(string url, object data, string mediaType, HttpMethod method, Action<HttpRequestMessage> callback = null) {
            HttpContent content = null;
            if (data != null) {
                content = ParseContext(mediaType, data);
            }
            var httpClient = _httpClientFactory.CreateClient(ServiceCollectionExtension.HttpClientName);
            EnsureInitialize(httpClient);
            httpClient.Timeout = new TimeSpan(0, 0, Timeout);
            var request = new HttpRequestMessage(method, url);
            
            callback?.Invoke(request);

            request.Content = content;

            return httpClient.Send(request);
        }

        [DebuggerStepThrough]
        private void EnsureInitialize(HttpClient client) {
            _initHandler?.Invoke(client);
        }

        [DebuggerStepThrough]
        private HttpContent ParseContext(string mediaType, object data) {
            var str = data as string;
            if (!str.IsNullOrEmpty()) {
                return new StringContent(str, Encoding.UTF8, mediaType);
            }
            var file = data as FileParameter;
            if (file != null) {
                return new StreamContent(file.Stream);
            }
            var stream = data as Stream;
            if (stream != null) {
                return new StreamContent(stream);
            }

            return null;
        }

        [DebuggerStepThrough]
        private CurlContext SendRequest(HttpMethod method, string url, string mediaType, object data, Action<HttpRequestMessage> callback = null) {
            var response = CreateRequest(url, data, mediaType, method, callback);
            return SendRequest(data, response);
        }

        private CurlContext SendRequest(object data, HttpResponseMessage response) {
            return new CurlContext {
                Request         = response.RequestMessage,
                Response        = response,
                Logger          = Logger,
                Body            = data
            };
        }
    }
}
