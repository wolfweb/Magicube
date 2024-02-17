using Magicube.Core;
using Magicube.Data.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.Web {
    using WebApplication = Microsoft.AspNetCore.Builder.WebApplicationBuilder;

    public static class ConfigurationExtensions {
        public static WebApplication AddConf<T>(this WebApplication builder, Action<ConfigurationProviderBuilder> handler) 
            where T : class, IConfigurationSource, IConfigurationSourceProvider, new() {
            var configure = new ConfigurationProviderBuilder(builder.Services);
            handler?.Invoke(configure);

            var source = New<T>.Instance();
            ((IConfigurationBuilder)builder.Configuration).Add(source);
            builder.Services.AddSingleton<IConfigurationSourceProvider>(source);
            source.Sources = configure.Sources;
            
            return builder;
        }

        public static IApplicationBuilder UseConf(this IApplicationBuilder builder) {
            var providers = builder.ApplicationServices.GetServices<IConfigurationSourceProvider>();
            var config = builder.ApplicationServices.GetService<IConfiguration>();
            foreach (var provider in providers) {
                provider.ServiceProvider = builder.ApplicationServices;
                provider.Load();
            }
            ((IConfigurationRoot)config).Reload();
            return builder;
        }
    }
}
