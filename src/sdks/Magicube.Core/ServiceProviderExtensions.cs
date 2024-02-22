using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.Core {
    public static class ServiceProviderExtensions {
        private static ConcurrentDictionary<Type, ObjectFactory> activatorCache = new();

        public static T GetService<T>(this IServiceProvider serviceProvider, string key) where T : class {
			var namedServiceType = NamedServiceProvider.GenerateNamedServiceType<T>(key);
			var namedService = serviceProvider.GetRequiredService(namedServiceType) as INamedService<T>;
			return namedService?.Service;
		}

        public static IEnumerable<T> GetServices<T>(this IServiceProvider serviceProvider, string key) where T : class {
            var namedServiceType = NamedServiceProvider.GenerateNamedServiceType<T>(key);
            var services = serviceProvider.GetServices(namedServiceType) as IEnumerable<INamedService<T>>;
            return services.Select(service => service.Service);
        }

        public static object GetService(this IServiceProvider serviceProvider, string key, Type type){
            var namedServiceType = NamedServiceProvider.GenerateNamedServiceType(key, type);
            var namedService = serviceProvider.GetRequiredService(namedServiceType) as INamedService<object>;
            return namedService?.Service;
        }

        public static T Resolve<T>(this IServiceProvider provider) { 
            var service = provider.GetService<T>();
            if(service == null){
                var factory = activatorCache.GetOrAdd(typeof(T), type => ActivatorUtilities.CreateFactory(type, Type.EmptyTypes));
                service = (T)factory(provider, null);
            }
            return service;
        }

        public static T Resolve<T>(this IServiceProvider provider, Type type, params object[] args) {
            var service = provider.GetService(type);
            if (service == null) {
                var factory = activatorCache.GetOrAdd(type, x => ActivatorUtilities.CreateFactory(x, args.Length == 0 ? Type.EmptyTypes : args.Select(x => x.GetType()).ToArray()));
                service = factory(provider, args);
            }
            if (service == null) return default;
            return (T)service;
        }

        public static T GetService<T>(this IServiceScope scope) {
            return scope.ServiceProvider.GetService<T>();
        }

        public static object GetService(this IServiceScope scope, Type type) {
            return scope.ServiceProvider.GetService(type);
        }

        public static IEnumerable<object> GetServices(this IServiceScope scope, Type type) {
            return scope.ServiceProvider.GetServices(type);
        }

        public static IEnumerable<T> GetServices<T>(this IServiceScope scope) {
            return scope.ServiceProvider.GetServices<T>();
        }
    }
}
