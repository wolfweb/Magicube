using COSXML;
using COSXML.CosException;
using COSXML.Model.Object;
using Magicube.Storage.Abstractions.Services;
using Magicube.Storage.Abstractions.Stores;
using Magicube.Storage.Tencent.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Magicube.Storage.Tencent.Services {
    public class TencentCloudStorageService : BaseCloudStorageService {
        public const string Key = "tencent";

        private readonly string _bucket;
        private readonly CosXml _cosClient;
        private readonly ILogger _logger;

        public TencentCloudStorageService(
            IFileStoreManage fileStoreService,
            ILogger<TencentCloudStorageService> logger,
            IStorageResolve<TencentStorageContext> storageResolve
            ) : base(fileStoreService) {
            _logger     = logger;
            var context = storageResolve.Resolve();
            _bucket     = context.Bucket;
            _cosClient  = context.CosClient;
        }

        public override string Identity => Key;

        protected override Task RemoveFile(string path) {
            try {
                DeleteObjectRequest request = new DeleteObjectRequest(_bucket, path);
                DeleteObjectResult result = _cosClient.DeleteObject(request);
                _logger.LogDebug(result.GetResultInfo());
            } catch (CosClientException cce) {
                _logger.LogError(cce.Message);
            } catch (CosServerException cse) {
                _logger.LogError(cse.Message);
            }
            return Task.CompletedTask;
        }
    }
}
