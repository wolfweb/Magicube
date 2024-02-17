using Magicube.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Magicube.Eventbus {
    public static  class ServiceCollectionExtension {
        public static IServiceCollection AddEventCore(this IServiceCollection services) {
            services.TryAddScoped<IEventProvider, EventProvider>();
            return services;
        }

        public static IServiceCollection AddEvent<T, TEvent>(this IServiceCollection services)
            where T : class
            where TEvent : class, IEvent<T> {
            services.AddTransient<IEvent<T>, TEvent>(typeof(T).Name);
            return services;
        }
    }
}
