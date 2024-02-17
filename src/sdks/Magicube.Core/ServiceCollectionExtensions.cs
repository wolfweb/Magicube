using Magicube.Core;
using Magicube.Core.Encrypts;
using Magicube.Core.Environment.Variable;
using Magicube.Core.Environment.Eventbus;
using Magicube.Core.EnvVariable;
using Magicube.Core.IO;
using Magicube.Core.PerformanceMonitor;
using Magicube.Core.Runtime;
using Magicube.Core.Signals;
using Magicube.Core.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Magicube.Core {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddCore(this IServiceCollection services, Action<MagicubeCoreBuilder> builder = null) {
            var coreBuilder = new MagicubeCoreBuilder(services);

            builder?.Invoke(coreBuilder);
            coreBuilder.Build();

            services.AddEnvVariable<GetDateVariableHandler>()
              .AddEnvVariable<GetFileNameVariableHandler>()
              .AddEnvVariable<GetPathNameVariableHandler>()
              .AddEnvVariable<GetDateTimeVariableHandler>()
              .AddEnvVariable<GetFileExtensionVariableHandler>();

            services.TryAddSingleton<CryptoServiceFactory>();
            services.TryAddTransient<FileAssistorProvider>();
            services.TryAddTransient<VariableFactroy>();

            services.TryAddSingleton(services);
            services.TryAddSingleton<Application>();
            services.TryAddSingleton<ISignal, Signal>();
            services.TryAddSingleton<GenUniqueShortString>();
            services.TryAddSingleton<RuntimeMetadataProvider>();
            services.TryAddSingleton<PerformanceMonitorFactory>();
            services.TryAddSingleton<IEventbus, EventbusProvider>();
            services.TryAddSingleton<IMapperProvider, MapperProvider>();
            services.TryAddTransient<IPerformanceMonitor, ConsolePerformanceMonitor>();

            services.AddOptions().AddLogging().AddHostedService<HostedBackgroundService>();
            
            return services;
        }

        public static IServiceCollection AddSingleton<T>(this IServiceCollection services, string key) {
            return services.AddSingleton<T, T>(key);

        }
        public static IServiceCollection AddSingleton<T, TImpl>(this IServiceCollection services, string key) {
            return Add(services, key, typeof(T), typeof(TImpl), ServiceLifetime.Singleton);
        }
        public static IServiceCollection AddScoped<T>(this IServiceCollection services, string key) {
            return services.AddScoped<T, T>(key);
        }
        public static IServiceCollection AddScoped<T, TImpl>(this IServiceCollection services, string key) {
            return Add(services, key, typeof(T), typeof(TImpl), ServiceLifetime.Scoped);
        }
        public static IServiceCollection AddTransient<T>(this IServiceCollection services, string key) {
            return services.AddTransient<T, T>(key);
        }
        public static IServiceCollection AddTransient<T, TImpl>(this IServiceCollection services, string key) {
            return Add(services, key, typeof(T), typeof(TImpl), ServiceLifetime.Transient);
        }
        public static IServiceCollection AddSingleton(this IServiceCollection services, string key, Type type, Type impl) {
            return Add(services, key, type, impl, ServiceLifetime.Singleton);
        }
        public static IServiceCollection AddScoped(this IServiceCollection services, string key, Type type, Type impl) {
            return Add(services, key, type, impl, ServiceLifetime.Scoped);
        }
        public static IServiceCollection AddTransient(this IServiceCollection services, string key, Type type, Type impl) {
            return Add(services, key, type, impl, ServiceLifetime.Transient);
        }
        
        public static IServiceCollection Add(this IServiceCollection services, string key, Type type, Type impl, ServiceLifetime serviceLifetime) {
            return Add(services, key, type,  provider => ActivatorUtilities.CreateInstance(provider, impl), serviceLifetime);
        }
        public static IServiceCollection Add(this IServiceCollection services, string key, Type type,  Func<IServiceProvider, object> implementationFactory, ServiceLifetime serviceLifetime) {
            var namedService = NamedServiceProvider.GenerateNamedServiceType(key, type);
            services.Add(new ServiceDescriptor(namedService, provider => Activator.CreateInstance(namedService, implementationFactory(provider)), serviceLifetime));
            return services;
        }

        public static IServiceCollection TryAddSingleton<T>(this IServiceCollection services, string key) {
            return services.TryAddSingleton<T, T>(key);
        }
        public static IServiceCollection TryAddSingleton<T, TImpl>(this IServiceCollection services, string key) {
            return TryAdd(services, key, typeof(T), typeof(TImpl), ServiceLifetime.Singleton);
        }
        public static IServiceCollection TryAddScoped<T>(this IServiceCollection services, string key) {
            return services.TryAddScoped<T, T>(key);
        }
        public static IServiceCollection TryAddScoped<T, TImpl>(this IServiceCollection services, string key) {
            return TryAdd(services, key, typeof(T), typeof(TImpl), ServiceLifetime.Scoped);
        }
        public static IServiceCollection TryAddTransient<T>(this IServiceCollection services, string key) {
            return services.TryAddTransient<T, T>(key);
        }
        public static IServiceCollection TryAddTransient<T, TImpl>(this IServiceCollection services, string key) {
            return TryAdd(services, key, typeof(T), typeof(TImpl), ServiceLifetime.Transient);
        }
        public static IServiceCollection TryAddSingleton(this IServiceCollection services, string key, Type type, Type impl) {
            return TryAdd(services, key, type, impl, ServiceLifetime.Singleton);
        }
        public static IServiceCollection TryAddScoped(this IServiceCollection services, string key, Type type, Type impl) {
            return TryAdd(services, key, type, impl, ServiceLifetime.Scoped);
        }
        public static IServiceCollection TryAddTransient(this IServiceCollection services, string key, Type type, Type impl) {
            return TryAdd(services, key, type, impl, ServiceLifetime.Transient);
        }

        public static IServiceCollection TryAdd(this IServiceCollection services, string key, Type type, Type impl, ServiceLifetime serviceLifetime) {
            return TryAdd(services, key, type, provider => ActivatorUtilities.CreateInstance(provider, impl), serviceLifetime);
        }
        public static IServiceCollection TryAdd(this IServiceCollection services, string key, Type type, Func<IServiceProvider, object> implementationFactory, ServiceLifetime serviceLifetime) {
            var namedService = NamedServiceProvider.GenerateNamedServiceType(key, type);
            if (services.All(x => x.ServiceType != namedService)) {
                services.Add(new ServiceDescriptor(namedService, provider => Activator.CreateInstance(namedService, implementationFactory(provider)), serviceLifetime));
            }

            return services;
        }

        public static IServiceCollection Replace<TService, TImplementation>(this IServiceCollection services) where TImplementation : TService {
            return services.Replace<TService>(typeof(TImplementation));
        }
        public static IServiceCollection Replace<TService>(this IServiceCollection services, Type implementationType) {
            return services.Replace(typeof(TService), implementationType);
        }
        public static IServiceCollection Replace(this IServiceCollection services, Type serviceType, Type implementationType) {
            if (services == null) {
                throw new ArgumentNullException(nameof(services));
            }

            if (serviceType == null) {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (implementationType == null) {
                throw new ArgumentNullException(nameof(implementationType));
            }

            if (!services.TryGetDescriptors(serviceType, out var descriptors)) {
                throw new ArgumentException($"No services found for {serviceType.FullName}.", nameof(serviceType));
            }

            foreach (var descriptor in descriptors) {
                var index = services.IndexOf(descriptor);

                services.Insert(index, descriptor.WithImplementationType(implementationType));

                services.Remove(descriptor);
            }

            return services;
        }

        public static IServiceCollection TryReplace<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient) where TImplementation : TService {
            return services.TryReplace<TService>(typeof(TImplementation), lifetime);
        }
        public static IServiceCollection TryReplace<TService>(this IServiceCollection services, Type implementationType, ServiceLifetime lifetime = ServiceLifetime.Transient) {
            return services.TryReplace(typeof(TService), implementationType, lifetime);
        }
        public static IServiceCollection TryReplace(this IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime lifetime = ServiceLifetime.Transient) {
            if (services == null) {
                throw new ArgumentNullException(nameof(services));
            }

            if (serviceType == null) {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (implementationType == null) {
                throw new ArgumentNullException(nameof(implementationType));
            }

            if (services.TryGetDescriptors(serviceType, out var descriptors)) {
                foreach (var descriptor in descriptors) {
                    var index = services.IndexOf(descriptor);

                    services.Insert(index, descriptor.WithImplementationType(implementationType));

                    services.Remove(descriptor);
                }
            } else {
                services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
            }

            return services;
        }

        public static IServiceCollection AddBackgroundService<T>(this IServiceCollection services) where T : class, IHostedService {
            services.AddHostedService<T>();
            return services;
        }

        public static IServiceCollection AddBackgroundTask<T>(this IServiceCollection services) where T : class, IBackgroundTask {
            services.AddSingleton<IBackgroundTask, T>();
            return services;
        }

        public static IServiceCollection AddConfigOptions<T>(this IServiceCollection services) where T : ShouldConfigOption {
            services.AddSingleton<T>();
            return services;
        }

        public static IServiceCollection AddEnvVariable<T>(this IServiceCollection services) where T : class, IVariableHandler {
            services.AddTransient<IVariableHandler, T>();
            return services;
        }

        private static bool TryGetDescriptors(this IServiceCollection services, Type serviceType, out ICollection<ServiceDescriptor> descriptors) {
            return (descriptors = services.Where(service => service.ServiceType == serviceType).ToArray()).Any();
        }

        private static ServiceDescriptor WithImplementationType(this ServiceDescriptor descriptor, Type implementationType) {
            return new ServiceDescriptor(descriptor.ServiceType, implementationType, descriptor.Lifetime);
        }
    }
}
