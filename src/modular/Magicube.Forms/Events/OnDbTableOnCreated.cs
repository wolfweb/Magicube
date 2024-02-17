using Magicube.Core.Models;
using Magicube.Core.Signals;
using Magicube.Data.Abstractions;
using Magicube.Eventbus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Magicube.Forms.Events {
    public class OnDbTableUpdatedEvent : OnUpdated<DbTable> {
        private readonly ILogger _logger;
        private readonly ISignal _signal;
        private readonly IMigrationManager _migrationManager;
        private readonly DynamicDataSourceOptions _dynamicDataSourceOptions;

        public OnDbTableUpdatedEvent(
            ISignal signal,
            ILogger<OnDbTableUpdatedEvent> logger,
            IOptions<DynamicDataSourceOptions> options,
            IMigrationManagerFactory migrationManagerFactory
            ) {
            _signal                   = signal;
            _logger                   = logger;
            _dynamicDataSourceOptions = options.Value;
            _migrationManager         = migrationManagerFactory.GetMigrationManager();
        }

        public override Task OnHandlingAsync(EventContext<DbTable> ctx) {
            if (ctx.Status.IsCancellationRequested) return Task.CompletedTask;

            if (ctx.Entity.Status == EntityStatus.Actived) {
                _logger.LogDebug($"rebuild table schema : {ctx.Entity.Name}");
                _migrationManager.BuildTable(ctx.Entity);
                _signal.SignalToken(_dynamicDataSourceOptions.DbTableSignalKey);
            }

            return Task.CompletedTask;
        }
    }
}
