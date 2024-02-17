using Magicube.Cache.Abstractions;
using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Web.Sites;

namespace Magicube.Caching.Services {
    class CacheConfigService : IMagicubeConfigProvider<CacheSetting> {
        private readonly ISiteManager _siteManager;
        private readonly IMapperProvider _mapperProvider;
        public CacheConfigService(
            ISiteManager siteManager, 
            IMapperProvider mapperProvider) {
            _siteManager       = siteManager;
            _mapperProvider    = mapperProvider;
        }

        public CacheSetting GetSetting() {
            var siteSettings = _siteManager.GetSite();
            CacheSetting cacheSettings = siteSettings.As<CacheSetting>(nameof(CacheSetting));
            if (cacheSettings != null) return cacheSettings;

            return new MemoryCacheSetting();
        }

        public void SetSetting(CacheSetting settings) {
            var siteSettings = _siteManager.GetSite();
            siteSettings.Put<CacheSetting>(nameof(CacheSetting), settings);
            _siteManager.UpdateSite(siteSettings);
        }
    }
}
