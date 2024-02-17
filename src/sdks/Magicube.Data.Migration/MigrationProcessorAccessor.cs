using FluentMigrator;
using FluentMigrator.Exceptions;
using FluentMigrator.Runner.Processors;
using Magicube.Core;
using Magicube.Data.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.Data.Migration {
    public class MigrationProcessorAccessor : IProcessorAccessor {
        private readonly MigrationOption _migrationOption;
        public MigrationProcessorAccessor(IOptionsSnapshot<MigrationOption> options, IEnumerable<IMigrationProcessor> processors) {
            _migrationOption = options.Value;
            ConfigureProcessor(processors);
        }

        protected virtual void ConfigureProcessor(IEnumerable<IMigrationProcessor> processors) {
            if (processors.Count() == 0) throw new ProcessorFactoryNotFoundException("No migration processor registered.");

            if (_migrationOption.ConnectionString.IsNullOrEmpty()) Processor = processors.FirstOrDefault();
            else {
                Processor = processors.FirstOrDefault(x => x.DatabaseType.Equals(_migrationOption.ConnectionProvider, StringComparison.OrdinalIgnoreCase) || x.DatabaseTypeAliases.Any(m => m.Equals(_migrationOption.ConnectionProvider, StringComparison.OrdinalIgnoreCase)));
            }
        }

        public IMigrationProcessor Processor { get; protected set; }
    }
}
