using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Storage.Abstractions.Entities;
using Magicube.Storage.Abstractions.Services;
using Magicube.Storage.Abstractions.Stores;
using Magicube.Storage.Abstractions.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Magicube.Storage.Abstractions {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddStorageCore(this IServiceCollection services) {
            AddEntities(services);

            services.TryAddSingleton<IStoragePathGenerator, StoragePathGenerator>();
            services.TryAddScoped<IStorageService, StorageService>(StorageService.Key);
            services.TryAddTransient<IFileStoreManage, FileStoreManage>();
            services.TryAddTransient<IStorageStoreManage, CloudStorageManage>();
            return services;
        }

        public static IServiceCollection AddStorageProvider<TView>(this IServiceCollection services, string provider) where TView : IStorageViewModel {
            services.Configure<StorageOptions>(x => {
                x.Register<TView>(provider);
            });
            return services;
        }

        private static void AddEntities(IServiceCollection services) {
            services.AddEntity<FileStore>();
            services.AddEntity<StorageStore, StorageStoreMapping>();
        }
    }
}
