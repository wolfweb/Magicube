using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Magicube.WebServer.RequestHandlers;
using Magicube.WebServer.Internal;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using Magicube.Core.Text;
using Newtonsoft.Json;

namespace Magicube.WebServer {
    public static class MiniWebContextExtensions {
        public static void WriteResponseObjectToResponseStream(this MiniWebContext context) {
            if (context.Response.ResponseStreamWriter != null)
                return;

            if (context.Response.ResponseObject == null)
                return;

            var streamResponse = context.Response.ResponseObject as Stream;

            if (streamResponse != null && streamResponse.Length >= 0) {
                context.Response.ResponseStreamWriter = stream => {
                    using (streamResponse)
                        streamResponse.CopyTo(stream);
                };

                return;
            }

            string serializedResponse = context.Configuration.SerializationService.Serialize(context.Response.ResponseObject);
            byte[] bytes = Encoding.UTF8.GetBytes(serializedResponse);

            using (var md5 = System.Security.Cryptography.MD5.Create()) {
                byte[] hash = md5.ComputeHash(bytes);
                var eTag = "\"" + BitConverter.ToString(hash).Replace("-", string.Empty) + "\"";
                context.Response.HeaderParameters["ETag"] = eTag;

                var requestETag = context.Request.HeaderParameters["If-None-Match"];
                if (string.IsNullOrWhiteSpace(requestETag) == false && requestETag.Equals(eTag, StringComparison.Ordinal)) {
                    context.Response.HttpStatusCode = Constants.HttpStatusCode.NotModified.ToInt();
                    return;
                }
            }

            context.Response.ResponseStreamWriter = stream => {
                stream.Write(bytes, 0, bytes.Length);
            };
        }

        public static MiniWebContext ReturnHttp404NotFound(this MiniWebContext context) {
            context.Handled = true;
            context.Response.HttpStatusCode = Constants.HttpStatusCode.NotFound.ToInt();
            context.Response.ContentType = "text/html";
            context.Response.ResponseStreamWriter = stream => stream.Write(Constants.CustomErrorResponse.NotFound404);
            return context;
        }

        public static void ReturnHttp500InternalServerError(this MiniWebContext context) {
            context.Handled = true;
            context.Response.HttpStatusCode = Constants.HttpStatusCode.InternalServerError.ToInt();
            context.Response.ResponseStreamWriter = context.WriteErrorsToStream;
        }

        private static void GenerateJsonErrorMessage(MiniWebContext context, Exception exception, StringBuilder stringBuilder) {
            using (var currentProcess = Process.GetCurrentProcess()) {
                var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                string totalRequestsHandled = "";
                string totalErrorsOccurred = "";

                if (context.IsRequestCounterEnabled())
                    totalRequestsHandled = RequestCounterHelper.RequestCount.ToString();

                if (context.IsErrorCounterEnabled())
                    totalErrorsOccurred = ErrorCounterHelper.ErrorCount.ToString();

                MiniWebError proxyError = null;

                WebException webException = exception as WebException;

                var stream = webException?.Response?.GetResponseStream();

                if (stream != null && webException.Status == WebExceptionStatus.ProtocolError) {
                    try {
                        using (StreamReader streamReader = new StreamReader(stream)) {
                            proxyError = JsonConvert.DeserializeObject<MiniWebError>(streamReader.ReadToEnd());
                        }
                    } catch {
                        proxyError = null;
                    }
                }

                string[] stackArray = Regex.Split(exception.StackTrace.Substring(3), @"(?= at )");

                var error = new MiniWebError {
                    Message = exception.Message,
                    ExceptionType = exception.GetType().FullName,
                    Data = new {
                        Stack = stackArray,
                        ExceptionData = exception.Data,
                        Request = new {
                            Url = context.Request.Url.SiteBase + context.Request.Url.Path,
                            FormBodyParameters = context.Request.FormBodyParameters
                        },
                        ProcessInfo = new {
                            StartupDateTime       = context.Configuration.ApplicationStartDateTime,
                            ApplicationUptime     = (DateTime.Now - context.Configuration.ApplicationStartDateTime).GetFormattedTime(),
                            TotalRequestsHandled  = totalRequestsHandled,
                            TotalErrorsOccurred   = totalErrorsOccurred,
                            MachineName           = ipGlobalProperties.HostName + "." + ipGlobalProperties.DomainName,
                            DomainUser            = Environment.UserDomainName + "\\" + Environment.UserName,
                            ApplicationDomainName = AppDomain.CurrentDomain.FriendlyName,
                            ProcessName           = currentProcess.ProcessName,
                            ProcessId             = currentProcess.Id.ToString()
                        },
                        InnerExceptions = proxyError != null ? (dynamic)proxyError : (dynamic)exception.InnerException
                    }
                };

                stringBuilder.Append(Json.Stringify(error));
            }
        }

        public static void WriteErrorsToStream(this MiniWebContext context, Stream stream) {
            context.Response.ContentType = "application/json";
            context.Response.HttpStatusCode = 500;

            if (context.Configuration.LogErrorsToEventLog) {
                var textErrorMessageBuilder = new StringBuilder();
                textErrorMessageBuilder.AppendLine("MiniWeb Error:");
                textErrorMessageBuilder.AppendLine("*************").AppendLine();

                foreach (Exception exception in context.Errors) {
                    context.WriteContextDataToExceptionData(exception);
                    EventLogHelper.GenerateTextErrorMessage(exception, textErrorMessageBuilder);
                }

                context.Configuration.WriteErrorToEventLog(textErrorMessageBuilder.ToString());
            }

            var serverHostName = Dns.GetHostName();
            var clientHostName = GetHostName(context.Request.ClientIpAddress);

            if (serverHostName == clientHostName || context.EnableVerboseErrors || context.Configuration.EnableVerboseErrors) {
                var jsonErrorMessageBuilder = new StringBuilder();

                foreach (Exception exception in context.Errors)
                    GenerateJsonErrorMessage(context, exception, jsonErrorMessageBuilder);

                var errorMessage = jsonErrorMessageBuilder.ToString();
                stream.Write(errorMessage);
                return;
            }

            stream.Write(Constants.CustomErrorResponse.InternalServerError500);
        }

        private static string GetHostName(string ipAddressString) {
            try {
                if (string.IsNullOrWhiteSpace(ipAddressString))
                    return "";
                var ipAddress = IPAddress.Parse(ipAddressString);
                var ipHostEntry = Dns.GetHostEntry(ipAddress);
                var hostNames = ipHostEntry.HostName.Split('.').ToList();
                return hostNames.FirstOrDefault();
            } catch (Exception) {
                return "";
            }
        }

        public static void WriteContextDataToExceptionData(this MiniWebContext context, Exception exception) {
            exception.Data["Request URL"]           = context.Request.Url.ToString();
            exception.Data["Request CorrelationId"] = context.CorrelationId;
            exception.Data["Request Timestamp"]     = context.RequestTimestamp.ToLocalTime();
            exception.Data["Client IP Address"]     = context.Request.ClientIpAddress;
            exception.Data["Current User"]          = context.CurrentUser == null ? "" : context.CurrentUser.UserName;
        }

        public static bool TryReturnFile(this MiniWebContext context, FileInfo fileInfo) {
            if (fileInfo.Exists) {
                context.Handled = true;
                context.Response.ContentType = FileExtensionToContentTypeConverter.GetContentType(fileInfo.Extension);
                var fileHash = DirectoryRequestHandler.GetFileHash(fileInfo);
                var eTag = "\"" + fileHash + "\""; 
                context.Response.HeaderParameters["ETag"] = eTag;
                context.Response.HeaderParameters["Last-Modified"] = fileInfo.LastWriteTimeUtc.ToString("R"); 

                var requestETag = context.Request.HeaderParameters["If-None-Match"];
                if (string.IsNullOrWhiteSpace(requestETag) == false && requestETag.Equals(eTag, StringComparison.Ordinal)) {
                    context.Response.HttpStatusCode = Constants.HttpStatusCode.NotModified.ToInt();
                    return true;
                }

                var requestDateString = context.Request.HeaderParameters["If-Modified-Since"];
                if (string.IsNullOrWhiteSpace(requestDateString) == false) {
                    DateTime requestDate;
                    if (DateTime.TryParseExact(requestDateString, "R", CultureInfo.InvariantCulture, DateTimeStyles.None, out requestDate)) {
                        if (((int)(fileInfo.LastWriteTimeUtc - requestDate).TotalSeconds) <= 0) {
                            context.Response.HttpStatusCode = Constants.HttpStatusCode.NotModified.ToInt();
                            return true;
                        }
                    }
                }

                context.Response.HeaderParameters["Content-Length"] = fileInfo.Length.ToString();

                if (fileInfo.Length > 0) {
                    context.Response.ResponseStreamWriter = stream => {
                        using (FileStream file = fileInfo.OpenRead())
                            file.CopyTo(stream, (int)(fileInfo.Length < Constants.DefaultFileBufferSize ? fileInfo.Length : Constants.DefaultFileBufferSize));
                    };
                }

                return true;
            }

            return false;
        }
    }

}
