using FluentMigrator;
using Magicube.Core;
using Magicube.Core.Models;
using Magicube.Data.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;

namespace Magicube.Data.Migration {
    public class DefaultMigration : AutoReversingMigration {
        private readonly IDbContext _dbContext;
        private readonly DatabaseOptions _options;
        private readonly IMigrationManager _migrationManager;
        private readonly IRepository<DbTable, int> _tableRepo;

        public DefaultMigration(
            IDbContext dbContext,
            IServiceProvider serviceProvider,
            IRepository<DbTable, int> tableRepo,
            IOptions<DatabaseOptions> dbOptions,
            IOptionsSnapshot<MigrationOption> options) {
            _tableRepo        = tableRepo;
            _options          = dbOptions.Value;
            _migrationManager = options.Value.Name.IsNullOrEmpty() ? serviceProvider.GetService<IMigrationManager>() :  serviceProvider.GetService<IMigrationManager>(options.Value.Name);

            _dbContext        = dbContext;
        }

        public override void Up() {
            foreach(var item in _options.EntityConfs) {
                _migrationManager.BuildTable(item.Key, _dbContext, item.Value != null);
            }

            if(Schema.Table(nameof(DbTable)).Exists()) {
                var tables = _tableRepo.Query(x => x.Status == EntityStatus.Actived);
                foreach (var table in tables) {
                    if (table.Fields == null) {
                        Trace.WriteLine($"dbtable=>{table.Name} has no fields");
                        continue;
                    }
                    _migrationManager.BuildTable(table);
                }
            } 
        }
    }
}
