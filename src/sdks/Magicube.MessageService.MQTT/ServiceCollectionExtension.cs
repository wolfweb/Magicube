using Microsoft.Extensions.DependencyInjection;

namespace Magicube.MessageService.MQTT {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddMqtt(this IServiceCollection services) {
            services
                .AddSingleton<IProduceProvider, MqttProducer>()
                .AddSingleton<IConsumerProvider, MqttConsumerProvider>();
            return services;
        }
    }
}
