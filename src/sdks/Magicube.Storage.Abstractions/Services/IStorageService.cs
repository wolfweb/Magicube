using Magicube.Core;
using Magicube.Core.Encrypts;
using Magicube.Core.IO;
using Magicube.Storage.Abstractions.Entities;
using Magicube.Storage.Abstractions.Stores;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;

namespace Magicube.Storage.Abstractions.Services {
    public interface IStorageService {
        string Identity { get; }
        Task<string> Get(string path);
        Task<bool> TryRemove(string path);
    }

    public interface IStorageOperateProvider<T> {
        string Save(string fileName, T model);
    }

    public abstract class BaseStorageService : IStorageService {
        private readonly IFileStoreManage _fileStoreService;

        protected BaseStorageService(IFileStoreManage fileStoreService) {
            _fileStoreService = fileStoreService;
        }

        public abstract string Identity { get; }

        public virtual Task<FileStore> Create(string name, string path, long length) {
            return _fileStoreService.CreateAsync(name, path, length);
        }

        public virtual async Task<string> Get(string path) {
            var entity = await _fileStoreService.GetAsync(path);
            return entity.Path;
        }

        public virtual async Task<bool> TryRemove(string path) {
            await _fileStoreService.DeleteAsync(path);
            await RemoveFile(path);
            return true;
        }

        protected abstract Task RemoveFile(string path);
    }

    public class StorageService : BaseStorageService, IStorageOperateProvider<Stream> {
        public const string Key = "default";
        private readonly StorageSetting _storageSetting;
        private readonly IWebFileProvider _webFileProvider;
        private readonly IStoragePathGenerator _pathGenerator;
        private readonly CryptoServiceFactory _cryptoServiceFactory;

        public StorageService(
            IOptionsMonitor<StorageSetting> options,
            IWebFileProvider webFileProvider,
            IFileStoreManage fileStoreService,
            IStoragePathGenerator pathGenerator,
            CryptoServiceFactory cryptoServiceFactory
            ) : base(fileStoreService) {
            _storageSetting = options.CurrentValue;
            _pathGenerator         = pathGenerator;
            _webFileProvider       = webFileProvider;
            _cryptoServiceFactory  = cryptoServiceFactory;
        }

        public override string Identity => Key;

        public string Save(string fileName, Stream stream) {
            var ext       = Path.GetFileNameWithoutExtension(fileName);
            var hashName  = _cryptoServiceFactory.GetCryptoService<ICryptoEncryptProvider>("MD5").Encrypt(stream.ReadAsBytes()).ToHexString();
            var _fileName = $"{hashName}{ext}";
            var path = Path.Combine(_webFileProvider.WebRootPath, _pathGenerator.Generate(_storageSetting.StorePathTemplate, _fileName));

            var dir = Path.GetDirectoryName(path);
            if(!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            Create(_fileName, path, stream.Length);

            using (var handler = File.Open(path, FileMode.Create)) {
                stream.CopyTo(handler);
                handler.Flush();
            }

            return path;
        }

        protected override Task RemoveFile(string path) {
            try {
                _webFileProvider.DeleteFile(Path.Combine(_webFileProvider.WebRootPath, path));
            } finally {

            }
            return Task.CompletedTask;
        }
    }
}
