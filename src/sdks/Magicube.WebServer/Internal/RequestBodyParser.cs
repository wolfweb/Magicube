using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Magicube.WebServer.Internal {
    internal static class RequestBodyParser {
        internal class RequestBodyParserResults {
            internal readonly IList<HttpFile> Files = new List<HttpFile>();
            internal readonly NameValueCollection FormBodyParameters = new NameValueCollection();
        }

        internal static RequestBodyParserResults ParseRequestBody(string contentType, Encoding contentEncoding, Stream requestBody, int parameterLimit) {
            var results = new RequestBodyParserResults();

            if (string.IsNullOrWhiteSpace(contentType))
                return results;

            string mimeType = contentType.Split(';').FirstOrDefault();

            if (string.IsNullOrWhiteSpace(mimeType))
                return results;

            if (mimeType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase)) {
                var sr = new StreamReader(requestBody, contentEncoding);
                string formData = sr.ReadToEnd();

                if (string.IsNullOrWhiteSpace(formData))
                    return results;

                string[] parameters = formData.Split('&');

                if (parameters.Length > parameterLimit)
                    throw new Exception("The limit of " + parameterLimit + " request parameters sent was exceeded. The reason to limit the number of parameters processed by the server is detailed in the following security notice regarding a DoS attack vector: http://www.ocert.org/advisories/ocert-2011-003.html");

                foreach (string parameter in parameters) {
                    if (parameter.Contains('=') == false)
                        throw new Exception("Can not parse the malformed form-urlencoded request parameters. Current parameter being parsed: " + parameter);

                    string[] keyValuePair = parameter.Split('=');
                    string decodedKey = UrlDecode(keyValuePair[0]);
                    string decodedValue = UrlDecode(keyValuePair[1]);
                    results.FormBodyParameters.Add(decodedKey, decodedValue);
                }

                return results;
            }

            if (!mimeType.Equals("multipart/form-data", StringComparison.OrdinalIgnoreCase))
                return results;

            var boundary = Regex.Match(contentType, @"boundary=""?(?<token>[^\n\;\"" ]*)").Groups["token"].Value;
            var multipart = new Multipart.HttpMultipart(requestBody, boundary);

            foreach (var httpMultipartBoundary in multipart.GetBoundaries(parameterLimit)) {
                if (string.IsNullOrEmpty(httpMultipartBoundary.Filename)) {
                    var reader = new StreamReader(httpMultipartBoundary.Value);
                    results.FormBodyParameters.Add(httpMultipartBoundary.Name, reader.ReadToEnd());
                } else
                    results.Files.Add(new HttpFile(httpMultipartBoundary.ContentType, httpMultipartBoundary.Filename, httpMultipartBoundary.Value, httpMultipartBoundary.Name));
            }

            requestBody.Position = 0;

            return results;
        }

        public static string UrlDecode(string text) {
            return Uri.UnescapeDataString(text.Replace("+", " "));
        }
    }
}
