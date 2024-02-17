using Magicube.Net.Email;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Magicube.Net {
    public static class ServiceCollectionExtension {
        internal const string HttpClientName = nameof(Magicube.Net);
        public static IServiceCollection AddHttpServices(this IServiceCollection services, Func<HttpMessageHandler> handler = null) {
            services
                .AddScoped<Curl>()
                .AddHttpContextAccessor()
                .AddHttpClient(HttpClientName)
                .ConfigurePrimaryHttpMessageHandler(x => {
                    return handler?.Invoke() ?? new HttpClientHandler {
                        AllowAutoRedirect = true
                    };
                });
            return services;
        }

        public static IServiceCollection AddEmailServices(this IServiceCollection services, Action<MailOption> action = null) {
            services.Configure<MailOption>(options=> {
                action?.Invoke(options);
            });
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IEmailConfigResolve, EmailConfigResolve>();
            return services;
        }
    }
}
