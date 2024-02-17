using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Medias.Nsfw {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddNfsw(this IServiceCollection services) {
            services.AddTransient<IMediaNsfwService, MediaNsfwService>();
            return services;
        }
    }
}