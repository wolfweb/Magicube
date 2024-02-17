using Magicube.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Magicube.Web.Middlewares {
    public class RequestLoggingMiddleware {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger) {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context) {
            var now = DateTime.UtcNow;
            var bodyEmpty = context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase) || context.Request.ContentLength.GetValueOrDefault(0L) == 0L;
            var originalStream = context.Request.Body;
            var sections = new List<string>();
            sections.Add($"{context.Request.Method} {UriHelper.GetDisplayUrl(context.Request)}");

            MemoryStream injectedStream = null;
            try {
                if (!bodyEmpty && !context.Request.ContentType.IsNullOrEmpty() && !context.Request.ContentType.Contains("multipart/form-data")) {
                    injectedStream = new MemoryStream(4096);
                    using (var requestReader = new StreamReader(context.Request.Body)) {
                        var requestBody = await requestReader.ReadToEndAsync();
                        if (!string.IsNullOrWhiteSpace(requestBody)) {
                            sections.Add($"body >> {requestBody}");
                        }
                        var requestBodyBytes = Encoding.UTF8.GetBytes(requestBody);
                        await injectedStream.WriteAsync(requestBodyBytes, 0, requestBodyBytes.Length);
                    }

                    injectedStream.Seek(0, SeekOrigin.Begin);
                    context.Request.Body = injectedStream;
                }
                await _next.Invoke(context);
                var time = DateTime.UtcNow.Subtract(now);
                sections[0] += string.Format("|{0:f0}", time.TotalMilliseconds);
                _logger.LogInformation(string.Join(System.Environment.NewLine, sections));
            } catch (Exception ex) {
                sections.Add($"Error >> {ex}");
                _logger.LogError(string.Join(System.Environment.NewLine, sections));
                throw ex;
            } finally {
                context.Request.Body = originalStream;
                if (injectedStream != null) {
                    injectedStream.Dispose();
                }
            }
        }
    }
}
