using Magicube.Data.Abstractions;
using Magicube.Storage.Abstractions.Entities;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Magicube.Storage.Abstractions.Stores {
    public interface IFileStoreManage {
        Task<FileStore> CreateAsync(string name, string path, long length);
        ValueTask<FileStore> GetAsync(string path);
        PageResult<FileStore, int> Paging(PageSearchModel model);
        Task DeleteAsync(string path);
    }

    public class FileStoreManage : IFileStoreManage {
        private readonly IRepository<FileStore, Guid> _repository;

        public FileStoreManage(IRepository<FileStore, Guid> repository) {
            _repository = repository;
        }

        public Task<FileStore> CreateAsync(string name, string path, long length) {
            var entity = new FileStore {
                Id       = Guid.Parse(Path.GetFileNameWithoutExtension(path)),
                Name     = name,
                CreateAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Length   = length,
                Path     = path,
            };
            return _repository.InsertAsync(entity);
        }

        public ValueTask<FileStore> GetAsync(string path) {
            var hash = Guid.Parse(Path.GetFileNameWithoutExtension(path));
            return _repository.GetAsync(hash);
        }

        public PageResult<FileStore, int> Paging(PageSearchModel model) {
            var query = _repository.All;
            var datas = query.Skip(model.PageSize * model.PageIndex++).Take(model.PageSize).ToArray();
            return new PageResult<FileStore, int>(model.PageIndex, datas);
        }

        public Task DeleteAsync(string path) {
            var hash = Guid.Parse(Path.GetFileNameWithoutExtension(path));
            return _repository.DeleteAsync(hash);
        }
    }
}
