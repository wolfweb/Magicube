using Magicube.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Storage.Tencent {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddTencentStorage(this IServiceCollection services) {
            services.AddStorageCore();
            return services;
        }
    }
}
