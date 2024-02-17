using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Magicube.Storage.Upyun.Models;
using System.Linq;
using Magicube.Core.Encrypts;

namespace Magicube.Storage.Upyun.Internal {
    public enum UpyunSecurityMode {
        Basic = 1,
        Md5 = 2,
        HamcSha1 = 3
    }

    class UpyunApi {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly NetworkCredential _networkCredential;
        private readonly CryptoServiceFactory _factory;

        private IUpyunSecurity _security;

        public const string DOMAIN = "v0.api.upyun.com";

        public string Bucket { get; set; }
        public TimeSpan? Timeout { get; set; }

        public UpyunApi(string username, string password, CryptoServiceFactory factory, IHttpClientFactory httpClientFactory) {
            _networkCredential = new NetworkCredential(username, password);
            _httpClientFactory = httpClientFactory;
            _factory = factory;
        }

        public void Initialize(string bucket, UpyunSecurityMode securityMode) {
            if (securityMode == UpyunSecurityMode.Basic) {
                _security = new BasicSecurity(_networkCredential, _factory);
            } else if (securityMode == UpyunSecurityMode.Md5) {
                _security = new Md5Security(_networkCredential, _factory);
            } else if (securityMode == UpyunSecurityMode.HamcSha1) {
                _security = new HamcSha1Security(_networkCredential, _factory);
            } else {
                throw new ArgumentOutOfRangeException(nameof(securityMode), $"UpyunSecurityMode {securityMode} not suported yet");
            }
            Bucket = bucket;
        }

        private async Task EnsureSuccessStatusCode(HttpResponseMessage response) {
            if (response.StatusCode != HttpStatusCode.OK) {
                string responseText = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(responseText)) {
                    throw new Exception(responseText);
                } else {
                    response.EnsureSuccessStatusCode();
                }
            }
        }

        private async Task<HttpResponseMessage> InvokeRequestAsync(HttpRequestMessage request) {
            var client = _httpClientFactory.CreateClient(nameof(UpyunApi));
            if (Timeout.HasValue) {
                client.Timeout = Timeout.Value;
            }
            client.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Keep-Alive", "600");
            await _security.Signature(request);

            var response = await client.SendAsync(request);
            await EnsureSuccessStatusCode(response);
            return response;
        }

        public async Task UploadFileAsync(string file, string path, string contentType = null) {
            var url = string.Format("http://{0}/{1}/{2}", DOMAIN, Bucket, path);
            using (var stream = File.OpenRead(file))
            using (var httpContent = new StreamContent(stream)) {
                if (!string.IsNullOrWhiteSpace(contentType)) {
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                }
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = httpContent;
                await InvokeRequestAsync(request);
            }
        }

        public async Task UploadFileAsync(byte[] buffer, string path, string contentType = null) {
            var url = string.Format("http://{0}/{1}/{2}", DOMAIN, Bucket, path);
            using (var httpContent = new ByteArrayContent(buffer)) {
                if (!string.IsNullOrWhiteSpace(contentType)) {
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                }
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = httpContent;
                await InvokeRequestAsync(request);
            }
        }

        public async Task<byte[]> GetFileAsync(string path) {
            var url = string.Format("http://{0}/{1}/{2}", DOMAIN, Bucket, path);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            using (var response = await InvokeRequestAsync(request)) {
                return await response.Content.ReadAsByteArrayAsync();
            }
        }

        public async Task<UpyunFile> HeadFileAsync(string path) {
            var url = string.Format("http://{0}/{1}/{2}", DOMAIN, Bucket, path);
            var request = new HttpRequestMessage(HttpMethod.Head, url);
            using (var response = await InvokeRequestAsync(request)) {
                var header = response.Headers;
                var file = new UpyunFile();
                file.Name = path;
                IEnumerable<string> values;
                if (header.TryGetValues("x-upyun-file-type", out values)) {
                    var value = values.FirstOrDefault();
                    if ("file".Equals(value, StringComparison.Ordinal)) {
                        file.Type = UpyunFileType.File;
                    } else if ("folder".Equals(value, StringComparison.Ordinal)) {
                        file.Type = UpyunFileType.Folder;
                    }
                }
                if (header.TryGetValues("x-upyun-file-size", out values)) {
                    var value = values.FirstOrDefault();
                    long size;
                    if (long.TryParse(value, out size)) {
                        file.Size = size;
                    }
                }
                if (header.TryGetValues("x-upyun-file-date", out values)) {
                    var value = values.FirstOrDefault();
                    double timestamp;
                    if (double.TryParse(value, out timestamp)) {
                        file.Date = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp);
                    }
                }
                return file;
            }
        }

        public async Task DeleteFileAsync(string path) {
            var url = string.Format("http://{0}/{1}/{2}", DOMAIN, Bucket, path);
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            using (var response = await InvokeRequestAsync(request)) {
                await response.Content.ReadAsStreamAsync();
            }
        }

        public async Task CreateFolderAsync(string path) {
            if (path.StartsWith("/")) {
                path = path.Substring(1);
            }
            var url = string.Format("http://{0}/{1}/{2}", DOMAIN, Bucket, path);
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("folder", "true");
            if (path.IndexOf('/') > 0) {
                request.Headers.Add("mkdir", "true");
            }
            using (var response = await InvokeRequestAsync(request)) {
                await response.Content.ReadAsStreamAsync();
            }
        }

        public async Task DeleteFolderAsync(string path) {
            var url = string.Format("http://{0}/{1}/{2}", DOMAIN, Bucket, path);
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            using (var response = await InvokeRequestAsync(request)) {
                await response.Content.ReadAsStreamAsync();
            }
        }

        public async Task<(IList<UpyunFile>, string)> ListFolderAsync(string path, int limit = 100, string iter = null) {
            var url = string.Format("http://{0}/{1}/{2}", DOMAIN, Bucket, path);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("x-list-limit", limit.ToString());
            if (!string.IsNullOrWhiteSpace(iter)) {
                request.Headers.Add("x-list-iter", iter);
            }

            using (var response = await InvokeRequestAsync(request)) {
                var responseText = await response.Content.ReadAsStringAsync();
                var lines = responseText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var files = new List<UpyunFile>(lines.Length);
                var cursor = response.Headers.GetValues("x-upyun-list-iter");

                foreach (var line in lines) {
                    var items = line.Split('\t');
                    if (items.Length != 4) {
                        Console.WriteLine("Parse faild for line \"{0}\"", line);
                        continue;
                    }
                    var file = new UpyunFile();
                    file.Name = items[0];
                    file.Type = items[1] == "N" ? UpyunFileType.File : UpyunFileType.Folder;
                    file.Size = long.Parse(items[2]);
                    file.Date = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(double.Parse(items[3]));
                    files.Add(file);
                }
                return (files, cursor.FirstOrDefault());
            }
        }

        public async Task<long> GetUsageAsync() {
            var url = string.Format("http://{0}/{1}/?usage", DOMAIN, Bucket);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            using (var response = await InvokeRequestAsync(request)) {
                var responseText = await response.Content.ReadAsStringAsync();
                return long.Parse(responseText);
            }
        }
    }
}