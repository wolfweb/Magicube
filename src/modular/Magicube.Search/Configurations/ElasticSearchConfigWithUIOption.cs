using Magicube.Core;
using Magicube.ElasticSearch;
using Magicube.Web.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Magicube.Search.Configurations {
    public class ElasticSearchConfigWithUIOption : ShouldConfigWithUIOption {
        private readonly ElasticSearchOptions _options;
        public ElasticSearchConfigWithUIOption(
            IOptionsMonitor<ElasticSearchOptions> options,
            IHttpContextAccessor httpContextAccessor
            ) : base(httpContextAccessor) {
            _options = options.CurrentValue;
        }

        public override RouteValueDictionary Routes => new RouteValueDictionary {
            ["area"]       = "Magicube.Search",
            ["action"]     = "Index",
            ["controller"] = "Admin",
        };

        public ElasticSearchOptions Option => _options;

        protected override bool OnConfigure() {
            var configProvider = GetService<IMagicubeConfigProvider<ElasticSearchOptions>>();
            var option         = configProvider.GetSetting();

            if (option == null) return false;

            _options.Hosts = option.Hosts;
            _options.Debug = option.Debug;
            return true;
        }
    }
}
