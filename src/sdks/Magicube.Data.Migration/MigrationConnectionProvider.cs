using FluentMigrator.Runner.Initialization;
using Magicube.Data.Abstractions;
using Microsoft.Extensions.Options;

namespace Magicube.Data.Migration {
    public class MigrationConnectionProvider : IConnectionStringAccessor {
        private readonly MigrationOption _migrationOption;
        public MigrationConnectionProvider(IOptionsSnapshot<MigrationOption> options) {
            _migrationOption = options.Value;
        }
        public string ConnectionString => _migrationOption.ConnectionString;
    }
}
