using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using Magicube.OpenIdCore.Stores;

namespace Magicube.OpenIdCore.Resolvers {
    public class OpenIdTokenStoreResolver : IOpenIddictTokenStoreResolver {
        private readonly TypeResolutionCache _cache;
        private readonly IServiceProvider _provider;

        public OpenIdTokenStoreResolver(TypeResolutionCache cache, IServiceProvider provider) {
            _cache = cache;
            _provider = provider;
        }

        public IOpenIddictTokenStore<TToken> Get<TToken>() where TToken : class {
            var store = _provider.GetService<IOpenIddictTokenStore<TToken>>();
            if (store != null) {
                return store;
            }

            var type = _cache.GetOrAdd(typeof(TToken), key => {
                return typeof(OpenIdTokenStore).MakeGenericType(key);
            });

            return (IOpenIddictTokenStore<TToken>)_provider.GetRequiredService(type);
        }
        public class TypeResolutionCache : ConcurrentDictionary<Type, Type> { }
    }
}
