using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using Magicube.OpenIdCore.Stores;

namespace Magicube.OpenIdCore.Resolvers {
    public class OpenIdAuthorizationStoreResolver : IOpenIddictAuthorizationStoreResolver {
        private readonly TypeResolutionCache _cache;
        private readonly IServiceProvider _provider;

        public OpenIdAuthorizationStoreResolver(TypeResolutionCache cache, IServiceProvider provider) {
            _cache = cache;
            _provider = provider;
        }

        public IOpenIddictAuthorizationStore<TAuthorization> Get<TAuthorization>() where TAuthorization : class {
            var store = _provider.GetService<IOpenIddictAuthorizationStore<TAuthorization>>();
            if (store != null) {
                return store;
            }

            var type = _cache.GetOrAdd(typeof(TAuthorization), key => {
                return typeof(OpenIdAuthorizationStore);
            });

            return (IOpenIddictAuthorizationStore<TAuthorization>)_provider.GetRequiredService(type);
        }

        public class TypeResolutionCache : ConcurrentDictionary<Type, Type> { }
    }
}
