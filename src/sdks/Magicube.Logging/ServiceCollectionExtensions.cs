using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;

namespace Magicube.Logging {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection UseNLog(this IServiceCollection services) {
            services.AddLogging(builder => {
                builder.AddNLog();
            });
            return services;
        }
    }
}
