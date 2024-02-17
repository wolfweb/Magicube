using Magicube.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.MessageService {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddMessageCore(this IServiceCollection services) {
            services.Configure<DefaultMessageOptions>(x => {

            });
            services.AddTransient<IMessageStore, NullProducerStore>();
            services.AddSingleton<IProduceProvider, DefaultProduceProvider>()
                .AddSingleton<IConsumerProvider, DefaultConsumerProvider>();
            return services;
        }

        public static IServiceCollection AddConsumer<T>(this IServiceCollection services, string key) where T : class, IConsumer {
            services.Configure<MessageOptions>(options => { 
                options.AddConsumer<T>(key);
            });
            return services;
        }
    }
}
