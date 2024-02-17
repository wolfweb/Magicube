using Magicube.Core;
using Magicube.Core.IO;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading;
using Magicube.Data.Abstractions;
using Magicube.Cache.Abstractions;
using System;

namespace Magicube.Web.Sites {
    public interface ISiteManager{
        DefaultSite GetSite();
        void SaveSite(DefaultSite site);
        void UpdateSite(DefaultSite site);
    }

    public class DefaultSiteManage : ISiteManager {
        private const string SiteConfigCacheKey = nameof(DefaultSiteManage);
        private const string SiteConfig = "~/App_Data/site.json";
        private readonly IWebFileProvider _webFileProvider;        
        private readonly DatabaseOptions _databaseOptions;
        private readonly IMapperProvider _mapperProvider;
        private readonly ICacheProvider _cacheProvider;

        private readonly ReaderWriterLockSlim _lockSlim;

        private readonly DefaultSite _Site;
        public DefaultSiteManage(
            IOptionsMonitor<DatabaseOptions> options,
            IOptionsMonitor<DefaultSite> siteOption,
            IServiceProvider serviceProvider,
            IWebFileProvider webFileProvider,
            IMapperProvider mapperProvider) {
            _Site            = siteOption.CurrentValue;
            _webFileProvider = webFileProvider;
            _databaseOptions = options.CurrentValue;
            _mapperProvider  = mapperProvider;
            _cacheProvider   = serviceProvider.GetService<ICacheProvider>(DefaultCacheProvider.Identity);

            _lockSlim        = new ReaderWriterLockSlim();
        }

        public DefaultSite GetSite() {
            _cacheProvider.GetOrAdd(SiteConfigCacheKey, () => {
                var conf = _webFileProvider.MapPath(SiteConfig);
                if (File.Exists(conf)) {
                    var site = File.ReadAllText(conf).JsonToObject<DefaultSite>();
                    _mapperProvider.Map(site, _Site);
                    _Site.Parts            = site.Parts;

                    if (!site.DatabaseProvider.IsNullOrEmpty()) {
                        _databaseOptions.Name = site.DatabaseProvider;
                    }
                    if (!site.ConnectionString.IsNullOrEmpty()) {
                        _databaseOptions.Value = site.ConnectionString;
                    }

                    return true;
                }
                return false;
            }, TimeSpan.FromMinutes(15));

            return _Site;
        }

        public void SaveSite(DefaultSite site) {
            var conf = _webFileProvider.MapPath(SiteConfig);
            var dir = Directory.GetParent(conf).FullName;
            
            _lockSlim.EnterWriteLock();
            
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            using(var writer = new StreamWriter(File.Open(conf, FileMode.OpenOrCreate))) {
                writer.Write(Json.Stringify(site));
            }

            if (!site.DatabaseProvider.IsNullOrEmpty()) {
                _databaseOptions.Name = site.DatabaseProvider;
            }
            if (!site.ConnectionString.IsNullOrEmpty()) {
                _databaseOptions.Value = site.ConnectionString;
            }

            _mapperProvider.Map(site, _Site);
            _cacheProvider.Remove<bool>(SiteConfigCacheKey);
            _lockSlim.ExitWriteLock();
        }

        public void UpdateSite(DefaultSite site) {
            var conf = _webFileProvider.MapPath(SiteConfig);
            var dir = Directory.GetParent(conf).FullName;
            
            _lockSlim.EnterWriteLock();

            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            using (var writer = new StreamWriter(File.Open(conf, FileMode.OpenOrCreate))) {
                var content = Json.Stringify(site);
                writer.Write(content);
                writer.Flush();
            }

            _mapperProvider.Map(site, _Site);
            _cacheProvider.Remove<bool>(SiteConfigCacheKey);
            _lockSlim.ExitWriteLock();
        }
    }
}
