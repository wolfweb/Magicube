using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using Magicube.Core;

namespace Magicube.Data.Abstractions.EfDbContext
{
    public class MagicubeDbContextFactory {
        private readonly DatabaseOptions _databaseOptions;
        private readonly IServiceProvider _serviceProvider;
        public MagicubeDbContextFactory(IOptionsMonitor<DatabaseOptions> options, IServiceProvider serviceProvider) {
            _databaseOptions = options.CurrentValue;
            _serviceProvider = serviceProvider;
        }

        public void Configure(DbContextOptionsBuilder builder) {
            var provider = _serviceProvider.GetService<IMagicubeDbContextProvider>(_databaseOptions.Name);
            provider.Configure(_databaseOptions, builder);
        }
    }
}
