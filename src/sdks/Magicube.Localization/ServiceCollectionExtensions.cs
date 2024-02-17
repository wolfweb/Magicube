using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Localization {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddFileLocalizerProvider<T>(this IServiceCollection services) where T : FileLocalizerProvider, ILocalizerProvider {
            services.AddSingleton<ILocalizerProvider, T>();
            return services;
        }
    }
}
