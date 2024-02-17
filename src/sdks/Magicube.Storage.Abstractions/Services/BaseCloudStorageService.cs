using Magicube.Storage.Abstractions.Stores;
using Magicube.Storage.Abstractions.ViewModels;
using System;
using System.IO;

namespace Magicube.Storage.Abstractions.Services {
    public abstract class BaseCloudStorageService : BaseStorageService, IStorageOperateProvider<SignatureViewModel> {
        protected BaseCloudStorageService(IFileStoreManage fileStoreService) : base(fileStoreService) {
        }

        public string Save(string fileName, SignatureViewModel model) {
            var uri = new Uri(model.SuccessUrl);
            Create(Path.GetFileName(uri.AbsolutePath), uri.AbsolutePath, model.Length);
            return model.SuccessUrl;
        }
    }
}
