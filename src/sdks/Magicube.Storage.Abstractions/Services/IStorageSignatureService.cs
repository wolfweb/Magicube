using Magicube.Core.Encrypts;
using Magicube.Storage.Abstractions.Entities;
using Magicube.Storage.Abstractions.Models;
using Magicube.Storage.Abstractions.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;

namespace Magicube.Storage.Abstractions.Services {
    /// <summary>
    /// 上传签名服务
    /// </summary>
    public interface IStorageSignatureService {
        SignatureViewModel GenerateWebUploadSignature(string uploadUrl, SignatureModel model, StorageStore storage);
    }

    public class StorageSignatureService : IStorageSignatureService {
        private readonly IUrlHelper _urlHelper;
        private readonly StorageOptions _storageOptions;
        private readonly CryptoServiceFactory _cryptoServiceFactory;
        public StorageSignatureService(
            IOptions<StorageOptions> options,
            IUrlHelperFactory urlHelperFactory,
            CryptoServiceFactory cryptoServiceFactory,
            IActionContextAccessor actionContextAccessor) {
            _storageOptions        = options.Value;
            _cryptoServiceFactory  = cryptoServiceFactory;
            _urlHelper      = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        public SignatureViewModel GenerateWebUploadSignature(string uploadUrl, SignatureModel model, StorageStore storage) {
            var cryptoService = _cryptoServiceFactory.GetCryptoService<ICryptoEncryptProvider>(Md5CryptoEncryptProvider.Identity);

            var args = $"{model.Provider}&{model.Method}&{model.Path}&{model.Expiration}";

            return new SignatureViewModel {
                Url     = uploadUrl,
                Headers = new System.Collections.Generic.Dictionary<string, string> { 
                    ["authorization"] = $"Magicube.Upload {cryptoService.Encrypt(args)}"
                },
            };
        }
    }
}
