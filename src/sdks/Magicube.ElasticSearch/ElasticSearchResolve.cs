using Microsoft.Extensions.Options;

namespace Magicube.ElasticSearch {
    public class ElasticSearchResolve : IElasticSearchResolve {
        private readonly ElasticSearchOptions _searchOptions;
        public ElasticSearchResolve(
            IOptionsMonitor<ElasticSearchOptions> options
            ) {
            _searchOptions = options.CurrentValue;
        }

        public ElasticSearchOptions Option => _searchOptions;
    }
}
