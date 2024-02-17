using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Magicube.Data.Abstractions {
    public interface IConfigurationSourceProvider {
        public IEnumerable<ConfigurationProviderItem> Sources { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
        public string Key { get; }
        void Load();
    }

    public class ConfigurationProviderBuilder {
        private readonly IServiceCollection _services;
        private readonly IList<ConfigurationProviderItem> _sources;

        public ConfigurationProviderBuilder(IServiceCollection services) {
            _services = services;
            _sources = new List<ConfigurationProviderItem>();
        }

        public ConfigurationProviderBuilder Add<T>(IConfiguration conf, Func<IDbContext, Dictionary<string, string>> func) where T : class {
            _services.Configure<T>(conf);
            _sources.Add(new ConfigurationProviderItem {
                Type = typeof(T),
                Build = func
            });
            return this;
        }

        public IEnumerable<ConfigurationProviderItem> Sources => _sources;
    }

    public class ConfigurationProviderItem {
        public Type Type { get; set; }
        public Func<IDbContext, Dictionary<string, string>> Build { get; set; }
    }
}
