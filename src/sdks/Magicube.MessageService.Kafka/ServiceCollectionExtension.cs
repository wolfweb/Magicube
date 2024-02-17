using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.MessageService.Kafka {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddKafka(this IServiceCollection services, Action<KafkaConfigureBuilder> builder) {
            var kafkaBuilder = new KafkaConfigureBuilder(services);
            builder?.Invoke(kafkaBuilder);

            services
                .AddSingleton<IProduceProvider, KafkaProducer>()
                .AddSingleton<IConsumerProvider, KafkaConsumerProvider>();
            return services;
        }
    }
}
