using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.MessageService.RabbitMQ {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddRabbitmq(this IServiceCollection services, Action<RabbitMessageBuilder> builder) {
            var rabbitBuilder = new RabbitMessageBuilder(services);

            builder(rabbitBuilder);

            services
                .AddSingleton<IRabbitConnectionFactory, RabbitConnectionFactory>()
                .AddSingleton<IProduceProvider, RabbitProducer>()
                .AddSingleton<IConsumerProvider, RabbitConsumerProvider>();
            return services;
        }
    }
}
