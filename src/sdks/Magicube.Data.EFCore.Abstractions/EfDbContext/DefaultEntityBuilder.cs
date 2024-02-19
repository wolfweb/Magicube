using Magicube.Core;
using Magicube.Core.Reflection;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Magicube.Data.Abstractions.EfDbContext {
    public class DefaultEntityBuilder : IEntityBuilder {
        private readonly DatabaseOptions _options;
        public DefaultEntityBuilder(IOptions<DatabaseOptions> options) {
            _options = options.Value;
        }

        public void Build(ModelBuilder modelBuilder) {
            foreach (var item in _options.EntityConfs) {
                var keys = TypeAccessor.Get(item.Key, null).Context.Properties.Where(x => x.GetAttribute<KeyAttribute>() != null).Select(x => x.Member.Name).ToArray();

                modelBuilder.Entity(item.Key, builder => {
                    builder.Ignore("Parts");
                    var attr = item.Key.GetCustomAttribute<TableAttribute>();
                    if (attr != null && !attr.Name.IsNullOrEmpty())
                        builder.ToTable(attr.Name);

                    if (keys.Any()) {
                        builder.HasKey(keys);
                    }
                });
                if(item.Value!= null) {
                    foreach (var type in item.Value) {
                        if (type == null) continue;
                        var conf = New<IMappingConfiguration>.Creator(type);
                        conf.ApplyConfiguration(modelBuilder);
                    }
                }
            }


            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())) {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
