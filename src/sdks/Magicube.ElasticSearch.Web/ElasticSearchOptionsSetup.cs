using Magicube.Data.Abstractions;
using Magicube.Web.Sites;
using Microsoft.Extensions.Options;

namespace Magicube.ElasticSearch.Web {
    public class ElasticSearchOptionsSetup : IConfigureOptions<ElasticSearchOptions> {
        private readonly ISiteManager _siteManager;

        public ElasticSearchOptionsSetup(ISiteManager siteManager) {
            _siteManager = siteManager;
        }

        public void Configure(ElasticSearchOptions options) {
            var searchSetting = _siteManager.GetSite().As<ElasticSearchOptions>();

            if(searchSetting != null) {
                options.Hosts = searchSetting.Hosts;
                options.Debug = searchSetting.Debug;
            }
        }
    }
}