using System.Net.Http;
using System.Threading.Tasks;
using Magicube.Core.Encrypts;
using Magicube.Storage.Abstractions.Services;
using Magicube.Storage.Abstractions.Stores;
using Magicube.Storage.Upyun.Internal;
using Magicube.Storage.Upyun.Models;

namespace Magicube.Storage.Upyun.Services {
    public class UpyunCloudStorageService : BaseCloudStorageService {
        public const string Key = "upyun";

        private readonly UpyunApi _upyunApi;
        public UpyunCloudStorageService(
            CryptoServiceFactory factory,
            IFileStoreManage fileStoreService,
            IHttpClientFactory httpClientFactory,
            IStorageResolve<UpyunStorageContext> cloudStorageResolve
            ) : base(fileStoreService) {
            var context = cloudStorageResolve.Resolve();
            _upyunApi = new UpyunApi(context.UserName, context.Password, factory, httpClientFactory);
            _upyunApi.Initialize(context.Bucket, UpyunSecurityMode.Md5);
        }

        public override string Identity => Key;

        protected override async Task RemoveFile(string path) {
            await _upyunApi.DeleteFileAsync(path);
        }
    }
}