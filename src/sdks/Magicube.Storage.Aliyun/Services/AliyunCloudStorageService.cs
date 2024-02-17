using Aliyun.OSS;
using Magicube.Storage.Abstractions.Services;
using Magicube.Storage.Abstractions.Stores;
using Magicube.Storage.Aliyun.Models;
using System.Threading.Tasks;

namespace Magicube.Storage.Aliyun.Services {
    public class AliyunCloudStorageService : BaseCloudStorageService {
        public static string Key = "aliyun";

        private readonly string _bucket;
        private readonly OssClient _ossClient;
        public AliyunCloudStorageService(
            IFileStoreManage fileStoreService,
            IStorageResolve<AliyunStorageContext> cloudStorageResolve
            ) : base(fileStoreService) {
            var context = cloudStorageResolve.Resolve();
            _bucket    = context.Bucket;
            _ossClient = context.Client;
        }

        public override string Identity => Key;

        protected override Task RemoveFile(string path) {
            _ossClient.DeleteObject(_bucket, path);
            return Task.CompletedTask;
        }
    }
}
