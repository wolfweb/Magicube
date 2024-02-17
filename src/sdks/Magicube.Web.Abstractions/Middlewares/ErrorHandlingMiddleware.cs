using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace Magicube.Web.Middlewares {
    public class ErrorHandlingMiddleware {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger) {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context) {
            int statusCode = 0;
            var message = string.Empty;
            try {
                await _next(context);
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);

                statusCode = SelectHttpStatusCode(ex);
                message = ex.Message;
            } finally {
                if (context.Response.ContentType?.IndexOf("json") > -1 && statusCode != 200 && message.Length > 0) {
                    await HandleExceptionAsync(context, statusCode, message);
                }
            }
        }

        private Task HandleExceptionAsync(HttpContext context, int statusCode, string msg) {
            var result = $"{{\"code\":{statusCode}, \"message\":\"{msg}\"}}";
            if (!context.Response.HasStarted)
                context.Response.ContentType = "application/json;charset=utf-8";
            return context.Response.WriteAsync(result);
        }

        private int SelectHttpStatusCode(Exception exception) {
            var apiException = exception as ApiException;
            if (apiException != null) {
                return (int)apiException.Code;
            } else if (exception is ValidationException) {
                return (int)HttpStatusCode.BadRequest;
            } else if (exception is InvalidOperationException) {
                const string authenticationSchemeMissing = "No authenticationScheme was specified, and there was no DefaultChallengeScheme found.";
                if (exception.Message == authenticationSchemeMissing) {
                    return (int)HttpStatusCode.Unauthorized;
                }
            }
            return (int)HttpStatusCode.InternalServerError;
        }
    }
}
