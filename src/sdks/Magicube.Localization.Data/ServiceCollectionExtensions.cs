using Magicube.Data;
using Magicube.Web;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.Localization.Data {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddStorageLocalizerProvider<T>(this IServiceCollection services) where T : StorageLocalizerProvider, ILocalizerProvider {
            services.AddSingleton<ILocalizerProvider, T>();
            return services;
        }
    }
}
