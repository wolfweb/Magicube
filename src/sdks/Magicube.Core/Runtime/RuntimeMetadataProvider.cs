using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.Core.Runtime {
    public class RuntimeMetadataProvider {
        private readonly IServiceCollection _services;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly StaticRuntimeMetadataOptions _runtimeMetadataOptions;
        private readonly ConcurrentBag<RuntimeMetadata> _metadatas = new ConcurrentBag<RuntimeMetadata>();
        public RuntimeMetadataProvider(IServiceCollection services, IServiceScopeFactory serviceScopeFactory, IOptions<StaticRuntimeMetadataOptions> options) {
            _services               = services;
            _serviceScopeFactory    = serviceScopeFactory;

            _runtimeMetadataOptions = options.Value;

            foreach (var item in _services.Where(x=> x.ImplementationType != null && typeof(IRuntimeMetadata).IsAssignableFrom(x.ImplementationType))) {
                _metadatas.Add(new RuntimeMetadata(item.ServiceType, item.ImplementationType));
            }

            foreach(var item in _runtimeMetadataOptions.ExportTypes) {
                _metadatas.Add(new RuntimeMetadata(item, null, true));
            }
        }

        public object Invoke(RuntimeMethodProvider methodProvider, params object[] args) {
            return methodProvider.RuntimeProvider.Invoke(methodProvider.Title, GetInstance(methodProvider), args);
        }

        public object GetInstance(RuntimeMethodProvider methodProvider) {
            if (methodProvider.IsStatic) return null;
            Type typeInterface;
            object typeInstance = null;

            using (var scope = _serviceScopeFactory.CreateScope()) {
                if (methodProvider.DeclaredType.IsInterface) {
                    if (methodProvider.DeclaredType.IsGenericType) {
                        var specialInterface = methodProvider.DeclaredType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRuntimeMetadata<>));
                        if (specialInterface != null) {
                            typeInterface = specialInterface.GetGenericArguments().First();
                            var services = scope.ServiceProvider.GetServices(typeInterface);
                            typeInstance = services.FirstOrDefault(x => x.GetType() == methodProvider.DeclaredType);
                        }
                    } else {
                        typeInstance = scope.ServiceProvider.GetService(methodProvider.DeclaredType);
                    }
                } else {
                    typeInterface = methodProvider.DeclaredType.GetInterfaces().FirstOrDefault(x => x != typeof(IRuntimeMetadata));
                    if (typeInterface != null) {
                        typeInstance = scope.ServiceProvider.GetService(typeInterface);
                    } else {
                        typeInstance = scope.ServiceProvider.GetService(methodProvider.DeclaredType);
                    }
                }
            }
             
            if (typeInstance == null) throw new Exception($"runtime method {methodProvider.Title} provider not found.");
            return typeInstance;
        }

        public IEnumerable<RuntimeMethodProvider> RuntimeMethods => _metadatas.SelectMany(x => x.Methods);
    }
}
