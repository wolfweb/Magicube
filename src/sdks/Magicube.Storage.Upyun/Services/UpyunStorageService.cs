using Magicube.Core;
using Magicube.Storage.Abstractions.Models;
using Magicube.Storage.Abstractions.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Magicube.Storage.Abstractions.Entities;
using Magicube.Storage.Abstractions.Services;

namespace Magicube.Storage.Upyun.Services {
    public class UpyunStorageService {
        private readonly IStoragePathGenerator _storagePathGenerator;
        public UpyunStorageService(IStoragePathGenerator storagePathGenerator) {
            _storagePathGenerator = storagePathGenerator;
        }

        public SignatureViewModel GenerateWebUploadSignature(string uploadUrl, SignatureModel model, StorageStore storage) {
            var dateTime = DateTimeOffset.UtcNow.ToString("r");
            var attr     = storage.Attribute.ToObject<CloudStorageStore>();
            var path     = _storagePathGenerator.Generate(storage.Template, model.Path);
            string policy = string.Empty;

            if (model.Expiration > 0) {
                var dict = new Dictionary<string, object>() {
                    ["bucket"]     = attr.Bucket,
                    ["save-key"]   = path,
                    ["expiration"] = model.Expiration,
                };
                if (model.Arg != null) {
                    foreach (var item in model.Arg) {
                        if (!dict.ContainsKey(item.Key)) {
                            dict.Add(item.Key, item.Value);
                        }
                    }
                }

                policy = Convert.ToBase64String(Encoding.UTF8.GetBytes(Json.Stringify(dict)));
            }
            var signature = ComputeSignature(model.Method.IsNullOrEmpty() ? "POST" : model.Method.ToUpper(), $"/{attr.Bucket}{path}", attr.AccessKey, model.Expiration == 0 ? dateTime : null, policy, null);

            return new SignatureViewModel {
                Url      = $"http://v0.api.upyun.com/{attr.Bucket}{path}",
                Name     = path,
                Headers  = new Dictionary<string, string> { },
                FormData = new Dictionary<string, string> {
                    ["authorization"] = $"UPYUN {attr.AccessKey}:{signature}",
                    ["policy"]        = policy
                },
                SuccessUrl = $"http://{attr.Host}{path}"
            };
        }

        private string ComputeSignature(string method, string uri, string operatorPwd, string expired = null, string policy = null, string contentMd5 = null) {
            var pwdMd5 = Md5Encrypt(operatorPwd);
            var rawStr = new StringBuilder($"{method}&{uri}");

            if (!expired.IsNullOrEmpty()) {
                rawStr.Append($"&{expired}");
            }

            if (!policy.IsNullOrEmpty()) {
                rawStr.Append($"&{policy}");
            }

            if (!contentMd5.IsNullOrEmpty()) {
                rawStr.Append($"&{contentMd5}");
            }

            return HmacSHA1(rawStr.ToString(), pwdMd5);
        }

        private string Md5Encrypt(string value) {
            using (var provider = MD5.Create()) {
                var bytes = provider.ComputeHash(Encoding.UTF8.GetBytes(value));
                return bytes.Aggregate("", (current, t) => current + t.ToString("x").PadLeft(2, '0'));
            }
        }

        private string HmacSHA1(string value, string key) {
            using (var provider = new HMACSHA1(Encoding.UTF8.GetBytes(key))) {
                var bytes = provider.ComputeHash(Encoding.UTF8.GetBytes(value));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
