using Magicube.Core;
using Magicube.Core.Models;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json.Linq;

namespace Magicube.Autoroute.Abstractions {
    public class AutorouteEntry : Entity {
        public long         ContentId { get; set; }

        [IndexField(IsUnique = true)]
        public string       Path      { get; set; }
                                      
        public EntityStatus Status    { get; set; }
                                      
        public JObject      Config    { get; set; }
    }

    public class AutorouteEntryMapping : EntityTypeConfiguration<AutorouteEntry> {
        public override void Configure(EntityTypeBuilder<AutorouteEntry> builder) {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Status).HasConversion(new EnumToNumberConverter<EntityStatus, int>());
            builder.Property(x => x.Config).HasConversion(x => Json.Stringify(x, null), x => x.JsonToObject<JObject>()).HasMaxLength(4000);
        }
    }
}
