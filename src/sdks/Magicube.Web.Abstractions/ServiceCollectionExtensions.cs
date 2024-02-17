using Magicube.Core;
using Magicube.Core.IO;
using Magicube.Eventbus;
using Magicube.Logging;
using Magicube.Web.Environment.Variable;
using Magicube.Web.Html;
using Magicube.Web.ModelBinders.Polymorphic;
using Magicube.Web.Navigation;
using Magicube.Web.Sites;
using Magicube.Web.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;

namespace Magicube.Web {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddWeb(this IServiceCollection services, Action<WebServiceBuilder> action = null) {
            services.UseNLog();
            services.AddCore();
            services.AddMemoryCache();
            services.Configure<DefaultSite>(options => { })
                .AddTransient<AccessTokenService>()
                .AddTransient<HttpServiceProvider>()
                .AddTransient(typeof(IMagicubeConfigProvider<>), typeof(BaseSiteModularConfigProvider<>))
                .AddSingleton<IRouteMatcher, RouteMatcher>()
                .AddSingleton<ISiteManager, DefaultSiteManage>()
                .AddSingleton<IWebFileProvider, MagicubeFileProvider>()
                .AddSingleton<IHtmlSanitizerService, HtmlSanitizerService>()
                .AddSingleton<IPermissionProvider, DefaultPermissionProvider>()
                .AddSingleton<IPolymorphicBindableModelBinderProvider, PolymorphicBindableModelBinderProvider>()
                .AddSingleton<IValidateOptions<PolymorphicModelBinderOptions>, PolymorphicModelBinderOptionsValidator>()
            ;

            services.TryAddScoped<INavigationManager, NavigationManager>();
            services.AddEventCore();

            services.AddOptions<PolymorphicModelBinderOptions>();

            AddEnvironmentVariables(services);

            var builder = new WebServiceBuilder(services);
            action?.Invoke(builder);
            builder.Build();

            return services;
        }

        public static IServiceCollection AddPermission(this IServiceCollection services, Assembly assembly, string display = "系统") {
            services.Configure<PermissionOptions>(options => {
                options.Add(display, assembly);
            });
            return services;
        }

        public static IServiceCollection AddPermission(this IServiceCollection services, string display, string area, Assembly assembly) {
            services.Configure<PermissionOptions>(options => {
                options.Add(display, assembly, area);
            });
            return services;
        }

        public static IServiceCollection AddPermission(this IServiceCollection services, params Permission[] permissions) {
            services.Configure<PermissionOptions>(options => {
                options.Add(permissions);
            });
            return services;
        }

        public static IServiceCollection AddPolymorphicModelBinder(this IServiceCollection services, Action<PolymorphicModelBinderOptions> options) {
            services.Configure<PolymorphicModelBinderOptions>(modelBinderOptions => {
                options?.Invoke(modelBinderOptions);
            });
            return services;
        }

        private static void AddEnvironmentVariables(IServiceCollection services) {
            services.AddEnvVariable<HttpMethodVariableHandler>();
            services.AddEnvVariable<HttpContentTypeVariableHandler>();
            services.AddEnvVariable<HttpRequestBodyVariableHandler>();
        }
    }
}
