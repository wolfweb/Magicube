using Magicube.Core;
using Magicube.Storage.Abstractions;
using Magicube.Storage.Abstractions.Services;
using Magicube.Storage.Upyun.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Storage.Upyun {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddUpyunStorage(this IServiceCollection services) {
            services.AddStorageCore();
            services.AddScoped<IStorageService, UpyunCloudStorageService>(UpyunCloudStorageService.Key);
            return services;
        }
    }
}
