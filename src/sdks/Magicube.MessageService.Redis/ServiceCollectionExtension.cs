using Microsoft.Extensions.DependencyInjection;

namespace Magicube.MessageService.Redis {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddRedisMessage(this IServiceCollection services) {
            services
                .AddSingleton<IProduceProvider, RedisMessageProducer>()
                .AddSingleton<IConsumerProvider, RedisMessageConsumerProvider>();
            return services;
        }
    }
}
