using Magicube.Cache.Abstractions;
using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Web.Sites;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.Cache.Web {
    public class CacheProviderFactory {
        private readonly IServiceProvider _serviceProvider;
        public CacheProviderFactory(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        public ICacheProvider GetCache() {
            var siteManager = _serviceProvider.GetService<ISiteManager>();
            var provider = siteManager.GetSite().As<CacheSetting>()?.CacheProvider ?? DefaultCacheProvider.Identity;
            return _serviceProvider.GetService<ICacheProvider>(provider);
        }
    }
}
