using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Eventbus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Magicube.LightApp.Abstractions {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddLightAppCore(this IServiceCollection services) {
            services.AddEntity<LightAppUserEntity, LightAppUserEntityMapping>();

            services.AddEvent<LightAppUserEntity, LightAppUserCreatingEvent>();
            services.AddEvent<LightAppUserEntity, LightAppUserUpdatingEvent>();

            services.TryAddSingleton<LightAppUserServiceFactory>();
            return services;
        }
    }
}
