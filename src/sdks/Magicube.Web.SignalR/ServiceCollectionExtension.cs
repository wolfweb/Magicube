using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Web.SignalR {
    public static class ServiceCollectionExtension {
        public static IServiceCollection UseSignalR(this IServiceCollection services) {
            services.AddSignalR(options => {
#if DEBUG
                options.EnableDetailedErrors = true;
#endif
                options.MaximumReceiveMessageSize = 100_000;
            });

            services.ConfigureOptions<SignalRConfigureSetup>();

            return services;
        }
    }
}
