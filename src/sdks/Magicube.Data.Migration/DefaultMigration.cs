using FluentMigrator;
using Magicube.Core;
using Magicube.Core.Models;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.EfDbContext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;

namespace Magicube.Data.Migration {
    public class DefaultMigration : AutoReversingMigration {
        private readonly IEntityBuilder _entityBuilder;
        private readonly IMigrationManager _migrationManager;
        private readonly IRepository<DbTable, int> _tableRepo;

        public DefaultMigration(
            IEntityBuilder entityBuilder,
            IServiceProvider serviceProvider,
            IRepository<DbTable, int> tableRepo,             
            IOptionsSnapshot<MigrationOption> options) {
            _tableRepo        = tableRepo;
            _entityBuilder    = entityBuilder;
            _migrationManager = options.Value.Name.IsNullOrEmpty() ? serviceProvider.GetService<IMigrationManager>() :  serviceProvider.GetService<IMigrationManager>(options.Value.Name);
        }

        public override void Up() {
            foreach(var type in _entityBuilder.Entities) {
                _migrationManager.BuildTable(type);
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
