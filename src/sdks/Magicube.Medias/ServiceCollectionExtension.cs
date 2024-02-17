using Magicube.Core;
using Magicube.Media.Fonts;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.Media {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddMediasCore(this IServiceCollection services, Action<MediaServiceBuilder> builder = null) {
            var mediaServiceBuilder = new MediaServiceBuilder();
            builder?.Invoke(mediaServiceBuilder);

            services.AddSingleton<FontProvider>();

            return services;
        }
    }
}
