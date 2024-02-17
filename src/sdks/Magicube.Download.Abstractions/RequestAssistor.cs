using Magicube.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Magicube.Download.Abstractions {
    public class RequestAssistor {
        private const string GetRequestMethod            = "GET";
        private const string HeaderContentRangeKey       = "Content-Range";
        private const string HeaderAcceptRangesKey       = "Accept-Ranges";
        private const string HeaderContentLengthKey      = "Content-Length";
        private const string HeaderContentDispositionKey = "Content-Disposition";

        private readonly Curl _curl;
        private readonly Regex _contentRangePattern;
        private readonly Dictionary<string, string> _responseHeaders;

        public RequestAssistor(string url, Curl curl) {
            _curl = curl;
            _responseHeaders = new Dictionary<string, string>();
            _contentRangePattern = new Regex(@"bytes\s*((?<from>\d*)\s*-\s*(?<to>\d*)|\*)\s*\/\s*(?<size>\d+|\*)", RegexOptions.Compiled);

            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri) == false) {
                uri = new Uri(new Uri("http://localhost"), url);
            }

            Address = uri;
        }

        public Uri Address { get; private set; }

        public async Task<long> GetFileSize() {
            if (await IsSupportDownloadInRange()) {
                return GetTotalSizeFromContentRange(_responseHeaders);
            }

            return GetTotalSizeFromContentLength(_responseHeaders);
        }

        public async Task<bool> IsSupportDownloadInRange() {
            await FetchResponseHeaders().ConfigureAwait(false);

            if (_responseHeaders.TryGetValue(HeaderAcceptRangesKey, out string acceptRanges) &&
                acceptRanges.ToLower() == "none") {
                return false;
            }

            if (_responseHeaders.TryGetValue(HeaderContentRangeKey, out string contentRange)) {
                if (!string.IsNullOrWhiteSpace(contentRange)) {
                    return true;
                }
            }

            return false;
        }

        public long GetTotalSizeFromContentRange(Dictionary<string, string> headers) {
            if (headers.TryGetValue(HeaderContentRangeKey, out string contentRange) && !string.IsNullOrWhiteSpace(contentRange)  && _contentRangePattern.IsMatch(contentRange)) {
                var match = _contentRangePattern.Match(contentRange);
                var size = match.Groups["size"].Value;

                return long.TryParse(size, out var totalSize) ? totalSize : -1L;
            }

            return -1L;
        }

        public long GetTotalSizeFromContentLength(Dictionary<string, string> headers) {
            if (headers.TryGetValue(HeaderContentLengthKey, out string contentLengthText) &&
                long.TryParse(contentLengthText, out long contentLength)) {
                return contentLength;
            }

            return -1L;
        }

        public async Task<string> GetFileName() {
            var filename = await GetUrlDispositionFilenameAsync().ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(filename)) {
                filename = GetFileNameFromUrl();
                if (string.IsNullOrWhiteSpace(filename)) {
                    filename = Guid.NewGuid().ToString("N");
                }
            }

            return filename;
        }

        public string GetFileNameFromUrl() {
            string filename = Path.GetFileName(Address.LocalPath);
            int queryIndex = filename.IndexOf("?", StringComparison.Ordinal);
            if (queryIndex >= 0) {
                filename = filename.Substring(0, queryIndex);
            }

            return filename;
        }

        public async Task<string> GetUrlDispositionFilenameAsync() {
            try {
                if (Address.IsWellFormedOriginalString() == true && Address.Segments.Length > 1) {
                    await FetchResponseHeaders().ConfigureAwait(false);
                    if (_responseHeaders.TryGetValue(HeaderContentDispositionKey, out string disposition)) {
                        string unicodeDisposition = ToUnicode(disposition);
                        if (!string.IsNullOrWhiteSpace(unicodeDisposition)) {
                            string filenameStartPointKey = "filename=";
                            string[] dispositionParts = unicodeDisposition.Split(';');
                            string filenamePart = dispositionParts.FirstOrDefault(part => part.Trim().StartsWith(filenameStartPointKey, StringComparison.OrdinalIgnoreCase));
                            if (!string.IsNullOrWhiteSpace(filenamePart)) {
                                string filename = filenamePart.Replace(filenameStartPointKey, "").Replace("\"", "").Trim();

                                return filename;
                            }
                        }
                    }
                }
            }
            catch (WebException e) {
                Debug.WriteLine(e);
            }

            return null;
        }

        public string ToUnicode(string otherEncodedText) {
            string unicode = Encoding.UTF8.GetString(Encoding.GetEncoding("iso-8859-1").GetBytes(otherEncodedText));
            return unicode;
        }

        public Uri GetRedirectUrl(HttpResponseMessage response) {
            var redirectLocation = response.Headers.Location?.ToString();
            if (!string.IsNullOrWhiteSpace(redirectLocation)) {
                return new Uri(redirectLocation);
            }

            return Address;
        }

        private async Task FetchResponseHeaders(bool addRange = true) {
            try {
                if (_responseHeaders.Any()) {
                    return;
                }
                var request = new HttpRequestMessage(HttpMethod.Get, Address);

                if (addRange) request.Headers.Range = new RangeHeaderValue(0, 0); // first byte

                using HttpResponseMessage response = _curl.Send(request).Response;
                EnsureResponseAddressIsSameWithOrigin(response);
                foreach (var item in response.Headers) {
                    if (response.Headers.TryGetValues(item.Key, out var headerValue)) {
                        _responseHeaders.Add(item.Key, string.Join(",", headerValue));
                    }
                }
                if(response.Content!=null) {
                    foreach (var item in response.Content.Headers) {
                        if (response.Content.Headers.TryGetValues(item.Key, out var headerValue)) {
                            _responseHeaders.Add(item.Key, string.Join(",", headerValue));
                        }
                    }
                }
            }
            catch (HttpRequestException exp)
            when (exp.StatusCode == HttpStatusCode.Found ||
                  exp.StatusCode == HttpStatusCode.Moved ||
                  exp.StatusCode == HttpStatusCode.MovedPermanently ||
                  exp.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable) {
                if (exp.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable) {
                    await FetchResponseHeaders(false).ConfigureAwait(false);
                }
            }
        }

        private bool EnsureResponseAddressIsSameWithOrigin(HttpResponseMessage response) {
            var redirectUri = GetRedirectUrl(response);
            if (!redirectUri.Equals(Address)) {
                Address = redirectUri;
                return false;
            }

            return true;
        }
    }
}