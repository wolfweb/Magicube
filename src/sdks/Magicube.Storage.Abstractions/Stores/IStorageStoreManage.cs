using Magicube.Data.Abstractions;
using Magicube.Storage.Abstractions.Entities;
using Magicube.Storage.Abstractions.ViewModels;
using System.Linq;

namespace Magicube.Storage.Abstractions.Stores {
    public interface IStorageStoreManage {
        StorageStore Create(CloudStorageViewModel model);
        PageResult<StorageStore, int> Paging(PageSearchModel model);
    }

    public class CloudStorageManage : IStorageStoreManage {
        private readonly IRepository<StorageStore, int> _cloudStorageRepository;

        public CloudStorageManage(IRepository<StorageStore, int> cloudStorageRepository) {
            _cloudStorageRepository = cloudStorageRepository;
        }

        public StorageStore Create(CloudStorageViewModel model) {
            return null;
        }

        public PageResult<StorageStore, int> Paging(PageSearchModel model) {
            if (model.PageIndex == -1) return PageResult<StorageStore, int>.Empty(-1);

            var query = _cloudStorageRepository.All;
            var datas = query.Skip(model.PageSize * model.PageIndex++).Take(model.PageSize).ToArray();
            return new PageResult<StorageStore, int>(model.PageIndex, datas);
        }
    }
}
