using Magicube.ElasticSearch;
using Magicube.Search.Configurations;
using Nest;

namespace Magicube.Search.Services {
	class ElasticSearchResolve : IElasticSearchResolve {
		private readonly ElasticSearchConfigWithUIOption _elasticSearchConfigWithUIOption;

		public ElasticSearchResolve(ElasticSearchConfigWithUIOption elasticSearchConfigWithUIOption) {
			_elasticSearchConfigWithUIOption = elasticSearchConfigWithUIOption;

			if (!_elasticSearchConfigWithUIOption.TryConfigure()) {
				_elasticSearchConfigWithUIOption.DoRedirect();
			}
		}

		public ElasticSearchOptions Option => _elasticSearchConfigWithUIOption.Option;
	}
}