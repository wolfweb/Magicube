using Magicube.Medias.Ocr;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Medias.Tools {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddOcr(this IServiceCollection services) {
            services.AddSingleton<IMagicubeMediaOcr, PaddleMediaOcr>();            
            return services;
        }
    }
}