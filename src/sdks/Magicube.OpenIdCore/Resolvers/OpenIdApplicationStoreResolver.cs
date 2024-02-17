using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using Magicube.OpenIdCore.Stores;

namespace Magicube.OpenIdCore.Resolvers {
    public class OpenIdApplicationStoreResolver : IOpenIddictApplicationStoreResolver {
        private readonly TypeResolutionCache _cache;
        private readonly IServiceProvider _provider;

        public OpenIdApplicationStoreResolver(TypeResolutionCache cache, IServiceProvider provider) {
            _cache = cache;
            _provider = provider;
        }

        public IOpenIddictApplicationStore<TApplication> Get<TApplication>() where TApplication : class {
            var store = _provider.GetService<IOpenIddictApplicationStore<TApplication>>();
            if (store != null) {
                return store;
            }

            var type = _cache.GetOrAdd(typeof(TApplication), key => {
                return typeof(OpenIdApplicationStore);
            });

            return (IOpenIddictApplicationStore<TApplication>)_provider.GetRequiredService(type);
        }

        public class TypeResolutionCache : ConcurrentDictionary<Type, Type> { }
    }
}
