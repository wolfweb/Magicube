using Magicube.Core;
using Magicube.Core.Reflection;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Magicube.Data.Abstractions.EfDbContext {
    public class DefaultEntityBuilder : IEntityBuilder {
        private readonly ConcurrentQueue<Type> _types = new ConcurrentQueue<Type>();
        private readonly ConcurrentQueue<Type> _configs = new ConcurrentQueue<Type>();

        public IEnumerable<Type> Entities => _types;

        public DefaultEntityBuilder(IOptions<DatabaseOptions> options) {
            foreach (var it in options.Value.Entities) {
                _types.Enqueue(it);
            }

            foreach (var it in options.Value.EntityMappings) {
                _configs.Enqueue(it);
            }
        }

        public void Build(ModelBuilder modelBuilder) {
            foreach (var type in _types) {
                var keys = TypeAccessor.Get(type, null).Context.Properties.Where(x => x.GetAttribute<KeyAttribute>() != null).Select(x => x.Member.Name).ToArray();

                modelBuilder.Entity(type, builder => {
                    builder.Ignore("Parts");
                    var attr = type.GetCustomAttribute<TableAttribute>();
                    if (attr != null && !attr.Name.IsNullOrEmpty())
                        builder.ToTable(attr.Name);

                    if (keys.Any()) {
                        builder.HasKey(keys);
                    }
                });

            }

            foreach (var type in _configs) {
                if (type == null) continue;
                var conf = New<IMappingConfiguration>.Creator(type);
                conf.ApplyConfiguration(modelBuilder);
            }

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())) {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
