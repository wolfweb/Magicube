using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using Magicube.OpenIdCore.Stores;

namespace Magicube.OpenIdCore.Resolvers {
    public class OpenIdScopeStoreResolver : IOpenIddictScopeStoreResolver {
        private readonly TypeResolutionCache _cache;
        private readonly IServiceProvider _provider;

        public OpenIdScopeStoreResolver(TypeResolutionCache cache, IServiceProvider provider) {
            _cache = cache;
            _provider = provider;
        }

        public IOpenIddictScopeStore<TScope> Get<TScope>() where TScope : class {
            var store = _provider.GetService<IOpenIddictScopeStore<TScope>>();
            if (store != null) {
                return store;
            }

            var type = _cache.GetOrAdd(typeof(TScope), key => {
                return typeof(OpenIdScopeStore);
            });

            return (IOpenIddictScopeStore<TScope>)_provider.GetRequiredService(type);
        }

        public class TypeResolutionCache : ConcurrentDictionary<Type, Type> { }
    }
}
