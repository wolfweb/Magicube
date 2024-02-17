using Magicube.Core;
using Magicube.Storage.Abstractions;
using Magicube.Storage.Abstractions.Services;
using Magicube.Storage.Aliyun.Services;
using Magicube.Storage.Aliyun.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Storage.Aliyun {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddAliyunStorage(this IServiceCollection services) {
            services.AddStorageCore();
            services.AddScoped<IStorageService, AliyunCloudStorageService>(AliyunCloudStorageService.Key);
            services.AddStorageProvider<AliyunStorageViewModel>(AliyunCloudStorageService.Key);
            return services;
        }
    }
}
