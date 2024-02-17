using Magicube.Core.Encrypts;
using Magicube.Storage.Abstractions.Entities;
using Magicube.Storage.Abstractions.Models;
using Magicube.Storage.Abstractions.Services;
using Magicube.Storage.Abstractions.ViewModels;
using System;
using System.Collections.Generic;

namespace Magicube.Storage.Tencent.Services {
    public class TencentStorageService : IStorageSignatureService {
        private readonly CryptoServiceFactory _cryptoServiceFactory;
        private readonly IStoragePathGenerator _storagePathGenerator;
        public TencentStorageService(
            CryptoServiceFactory cryptoServiceFactory,
            IStoragePathGenerator storagePathGenerator 
            ) {
            _storagePathGenerator = storagePathGenerator;
            _cryptoServiceFactory = cryptoServiceFactory;
        }

        public SignatureViewModel GenerateWebUploadSignature(string uploadUrl, SignatureModel model, StorageStore storage) {
            var path = _storagePathGenerator.Generate(storage.Template, model.Path);
            var attr = storage.Attribute.ToObject<CloudStorageStore>();
            var signature = FormatSignatureStr(path, attr);

            return new SignatureViewModel {
                Url = $"http://{attr.Host}{path}",
                Headers = new Dictionary<string, string> {
                    ["Authorization"] = $"{signature}"
                },
                SuccessUrl = ""
            };
        }

        private string FormatSignatureStr(string filename, CloudStorageStore storage, string method = "put") {
            method = method.ToLower();
            if(!filename.StartsWith('/'))
                filename = "/" + filename;
            
            var baseDate = new DateTime(1970, 1, 1);
            var now = (int)(DateTime.Now.AddDays(-5) - baseDate).TotalSeconds;
            var exp = (int)(DateTime.Now.AddMinutes(2) - baseDate).TotalSeconds;
            string signTime         = now + ";" + exp;
            string keyTime          = signTime;
            string singKey          = HmacSha1(keyTime, storage.AccessKey);
            string httpString       = method + "\n" + filename + "\n\n\n";
            string sha1edHttpString = Sha1(httpString);
            string stringToSign     = $"sha1\n{signTime}\n{sha1edHttpString}\n";
            string signature        = HmacSha1(stringToSign, singKey);
            var authorization       = $"q-sign-algorithm=sha1&q-ak={storage.AccessSecret}&q-sign-time={signTime}&q-key-time={signTime}&q-header-list=&q-url-param-list=&q-signature={signature}";
            return authorization;
        }

        private string Sha1(string str) {
            var sha1 = _cryptoServiceFactory.GetCryptoService<ICryptoEncryptProvider>(Sha1CryptoEncryptProvider.Identity);
            return sha1.Encrypt(str).ToLower();
        }

        private string HmacSha1(string EncryptText, string EncryptKey) {
            var hmac = _cryptoServiceFactory.GetCryptoService<ICryptoEncryptProvider>(HmacSha1CryptoEncryptProvider.Identity, EncryptKey);
            return hmac.Encrypt(EncryptText).ToLower();
        }
    }
}
