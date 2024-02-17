using Magicube.Storage.Abstractions.ViewModels;
using Microsoft.AspNetCore.Routing;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Magicube.Storage.Abstractions {
    public class StorageOptions {
        private readonly ConcurrentDictionary<string, object> _providerWithViews = new();
        public StorageOptions() {
            
        }

        public StorageOptions Register<T>(string provider) where T : IStorageViewModel {
            _providerWithViews.AddOrUpdate(provider, key => typeof(T), (key, v) => typeof(T));
            return this;
        }

        public RouteValueDictionary               UploadWithAuthorizeRoute   { get; set; }
        public RouteValueDictionary               UploadNoAuthorizeRoute     { get; set; }

        public IReadOnlyDictionary<string,object> ProviderViews => _providerWithViews.ToImmutableDictionary();
    }
}
