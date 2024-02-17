using Magicube.Core.Modular;
using Magicube.ElasticSearch;
using Magicube.ElasticSearch.Web;
using Magicube.ElasticSearch7;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Magicube.Web {
    public static class WebServiceBuilderExtension {
        public static WebServiceBuilder AddSearchWeb(this WebServiceBuilder builder, Action<ElasticSearchOptions> configure = null) {
            builder.Services
                .Configure<ModularOptions>(options => {
                    options.ModularShareAssembly.Add(typeof(ElasticSearchOptionsSetup).Assembly);
                })
                .AddElasticSearchServices(configure);
            return builder;
        }
    }
}