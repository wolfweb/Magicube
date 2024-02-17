using Magicube.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Magicube.Data.Abstractions {
	public interface IMigrationManagerFactory {
		IMigrationManager GetMigrationManager(MigrationType type = MigrationType.Application);
	}

	public class MigrationManagerFactory : IMigrationManagerFactory {
		private readonly IServiceProvider _serviceProvider;
		public MigrationManagerFactory(IServiceProvider serviceProvider) {
			_serviceProvider = serviceProvider;
		}

		public IMigrationManager GetMigrationManager(MigrationType type) {
			switch (type) {
				case MigrationType.Application:
					return GetAppMigrationManager();
				case MigrationType.Modular:
					throw new NotImplementedException();
				default:
					return GetAppMigrationManager();
			}
		}

		private IMigrationManager GetAppMigrationManager() {
			var options = _serviceProvider.GetService<IOptionsMonitor<DatabaseOptions>>().CurrentValue;
			var migrationOption = _serviceProvider.GetService<IOptionsSnapshot<MigrationOption>>().Value;
			migrationOption.Name = options.Name;
			migrationOption.ConnectionString = options.Value;
			migrationOption.ConnectionProvider = options.Name;

			return options.Name.IsNullOrEmpty() ? _serviceProvider.GetService<IMigrationManager>() : _serviceProvider.GetService<IMigrationManager>(options.Name);
		}
	}
}
