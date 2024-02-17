using Magicube.Core;
using Magicube.Versioning.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Magicube.Versioning.Filebase {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddVersioning(this IServiceCollection services, VersioningOption option = null) {
            if(option != null) {
                services.Configure<VersioningOption>(options => { 
                    options.Folder    = option.Folder;
                    options.UserName  = option.UserName;
                    options.UserEmail = option.UserEmail;
                });
            }
            services.TryAddTransient<IVersioningProvider, VersioningProvider>();
            services.TryAddScoped<IVersioningMessageService, NullVersioningMessageService>();
            return services;
        }
    }
}