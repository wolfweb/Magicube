using Magicube.Core;
using Magicube.Core.Runtime;
using Magicube.Core.Runtime.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Magicube.Web {
    public class HttpServiceProvider : IRuntimeMetadata {
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HttpServiceProvider(IUrlHelperFactory urlHelperFactory, IHttpContextAccessor httpContextAccessor) {
            _urlHelperFactory    = urlHelperFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        [RuntimeMethod]
        public object QueryString(string key) {
            if(key.IsNullOrEmpty()) return _httpContextAccessor.HttpContext.Request.QueryString.ToString();

            object result;
            if (_httpContextAccessor.HttpContext.Request.Query.TryGetValue(key, out var values)) {
                if (values.Count == 0) {
                    result = null;
                } else if (values.Count == 1) {
                    result = values[0];
                } else {
                    result = values.ToArray();
                }
            } else {
                result = null;
            }
            return result;
        }

        [RuntimeMethod]
        public void ResponseWrite(string text) {
            _httpContextAccessor.HttpContext.Response.WriteAsync(text).GetAwaiter().GetResult();
        }

        [RuntimeMethod]
        public string AbsoluteUrl(string relativePath) {
            var urlHelper = _urlHelperFactory.GetUrlHelper(new ActionContext(_httpContextAccessor.HttpContext, new RouteData(), new ActionDescriptor()));
            return urlHelper.ToAbsoluteUrl(relativePath);
        }

        [RuntimeMethod]
        public string ReadBody() {
            using (var sr = new StreamReader(_httpContextAccessor.HttpContext.Request.Body)) {
                return sr.ReadToEndAsync().GetAwaiter().GetResult();
            }
        }

        [RuntimeMethod]
        public object RequestForm(string field) {
            object result;
            if (_httpContextAccessor.HttpContext.Request.Form.TryGetValue(field, out var values)) {
                if (values.Count == 0) {
                    result = null;
                } else if (values.Count == 1) {
                    result = values[0];
                } else {
                    result = values.ToArray();
                }
            } else {
                result = null;
            }
            return result;
        }

        [RuntimeMethod]
        public JObject QueryStringAsJson() {
            return new JObject((from param in _httpContextAccessor.HttpContext.Request.Query
                                select new JProperty(param.Key, JArray.FromObject(param.Value.ToArray()))).ToArray());
        }

        [RuntimeMethod]
        public JObject RequestFormAsJson() {
            return new JObject((from field in _httpContextAccessor.HttpContext.Request.Form
                                select new JProperty(field.Key, JArray.FromObject(field.Value.ToArray()))).ToArray());
        }

        [RuntimeMethod]
        public string GetBody() {
            using (var reader = new StreamReader(_httpContextAccessor.HttpContext?.Request.Body)) {
                return reader.ReadToEnd();
            }
        }
        public string Body => GetBody();

        [RuntimeMethod]
        public string GetUrlReferrer() => UrlReferrer;
        public string UrlReferrer => _httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Referer];

        [RuntimeMethod]
        public string GetUserAgent() => UserAgent;
        public string UserAgent => _httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.UserAgent];

        [RuntimeMethod]
        public string GetContentType() => ContentType;
        public string ContentType => _httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.ContentType];

        [RuntimeMethod]
        public string GetUrl() => Url;
        public string Url => _httpContextAccessor.HttpContext?.Request.GetDisplayUrl();

        [RuntimeMethod]
        public string GetMethod() => Method;
        public string Method => _httpContextAccessor.HttpContext?.Request.Method;

        [RuntimeMethod]
        public string ClientIp() {
            var result = _httpContextAccessor.HttpContext?.Request?.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(result) && _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress != null) {
                if (_httpContextAccessor.HttpContext.Connection.RemoteIpAddress.IsIPv4MappedToIPv6)
                    result = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress.MapToIPv4().ToString();
                else
                    result = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress.ToString();
            }
            return result;
        }

        [RuntimeMethod]
        public List<IFormFile> GetFiles() {
            var result = new List<IFormFile>();
            var files = _httpContextAccessor.HttpContext?.Request.Form.Files;
            if (files == null || files.Count == 0)
                return result;
            result.AddRange(files.Where(file => file?.Length > 0));
            return result;
        }

        [RuntimeMethod]
        public IFormFile GetFile() {
            var files = GetFiles();
            return files.Count == 0 ? null : files[0];
        }

        [RuntimeMethod]
        public async Task DownloadAsync(Stream stream, string fileName) {
            await DownloadAsync(stream, fileName, Encoding.UTF8);
        }

        [RuntimeMethod]
        public async Task DownloadAsync(Stream stream, string fileName, Encoding encoding) {
            await DownloadAsync(stream.ReadAsBytes(), fileName, encoding);
        }

        [RuntimeMethod]
        public async Task DownloadAsync(byte[] bytes, string fileName, Encoding encoding) {
            if (bytes == null || bytes.Length == 0)
                return;
            fileName = fileName.Replace(" ", "");
            fileName = HttpUtility.UrlEncode(fileName, encoding);
            _httpContextAccessor.HttpContext.Response.ContentType = "application/octet-stream";
            _httpContextAccessor.HttpContext.Response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
            await _httpContextAccessor.HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }

        [RuntimeMethod]
        public bool IsStaticResource() {
            string path = _httpContextAccessor.HttpContext.Request.Path;
            var contentTypeProvider = new FileExtensionContentTypeProvider();
            return contentTypeProvider.TryGetContentType(path, out string _);
        }

        [RuntimeMethod]
        public void AddOrUpdate<T>(string key, T value) {
            if (_httpContextAccessor.HttpContext.Items.ContainsKey(key))
                _httpContextAccessor.HttpContext.Items[key] = value;
            else
                _httpContextAccessor.HttpContext.Items.Add(key, value);
        }

        [RuntimeMethod]
        public T Get<T>(string key) {
            if (_httpContextAccessor.HttpContext.Items.TryGetValue(key, out object obj)) {
                return (T)obj;
            }
            return default;
        }
    }
}
