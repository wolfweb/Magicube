using Microsoft.Extensions.DependencyInjection;
using Magicube.ElasticSearch;
using System;

namespace Magicube.ElasticSearch7 {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddElasticSearchServices(this IServiceCollection services, Action<ElasticSearchOptions> configure = null) {
            services.Configure<ElasticSearchOptions>(option => {
                configure?.Invoke(option);
            });
            services.AddSingleton<IElasticSearchResolve, ElasticSearchResolve>();
            services.AddSingleton(typeof(IElastic7SearchService), typeof(ElasticSearchService));
            return services;
        }
    }
}
