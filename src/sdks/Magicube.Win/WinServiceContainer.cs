using Magicube.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Magicube.Win {
    public class WinServiceContainer {
        private readonly IServiceCollection _services;
        private Action<MagicubeCoreBuilder> _coreBuilder;

        public IConfiguration Configuration { get; }

        public WinServiceContainer(EnvironmentName environmentName) {
            _services = new ServiceCollection();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true);
            Configuration = builder.Build();
        }

        public WinServiceContainer SetupCore(Action<MagicubeCoreBuilder> builder) {
            _coreBuilder = builder;
            return this;
        }

        public WinServiceContainer Setup(Action<IServiceCollection, IConfiguration> action) {
            action?.Invoke(_services, Configuration);
            _services.AddCore(_coreBuilder);
            return this;
        }

        public IServiceProvider Build() {
            var result = _services.BuildServiceProvider();
            result.GetService<Application>().ServiceProvider = result;
            return result;
        }
    }

    public enum EnvironmentName {
        Development,
        Production
    }
}
