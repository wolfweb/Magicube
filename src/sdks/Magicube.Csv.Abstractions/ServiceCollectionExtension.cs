using CsvHelper.Configuration;
using Magicube.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Magicube.Csv.Abstractions {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddCsvParse(this IServiceCollection services) {
            services.TryAddSingleton<CsvParseFactory>();
            return services;
        }

        public static IServiceCollection RegisterCsvMap<T>(this IServiceCollection services) where T : ClassMap {
            services.Configure<CsvParseOption>(x=>x.RetisterMap<T>());
            return services;
        }
    } 
}