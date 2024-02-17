using Magicube.AutoMap.Mapster;
using Magicube.Core.Modular;
using Magicube.Modular.Web.Extensions;
using Magicube.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.Web {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddMagicubeWeb(this IServiceCollection services, Action<ModularOptions> conf) {
            services.Configure(conf);
            services.AddWeb(builder => {
                builder
                .AddMvc(options => {
                    //options.OutputFormatters.Add();
                    //options.InputFormatters.Add();
                })
                .AddSecurity()
                .AddJsonOptions()
                .AddCors()
                .AddIdentityWeb(builder => {
                    builder.Configure(identityOption => {
                        identityOption.Password.RequireLowercase = false;
                        identityOption.Password.RequireUppercase = false;
                        identityOption.Password.RequireNonAlphanumeric = false;
                    });
                })
                .AddCache()
                .AddModular()
                .AddStorageLocalizer()
                .AddResourceManagement()
                .AddStorageWeb()
                .AddAutoRoute()
                .AddDatabases()
                .AddSearchWeb()
                .AddWebUICore()
                .AddSignalR()
                .AddLiquid()
                .AddQuartz()
                ;
            });

            services.AddAutoMapMapster();
            return services;
        }
    }
    
    public static class ApplicationBuilderExtension {
        public static IApplicationBuilder UseMagicubeWeb(this IApplicationBuilder app) {
            app.UseWeb(builder => {
                builder.UseModular();
                builder.UseSignalR();
            });
            return app;
        }
    }
}
