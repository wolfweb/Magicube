using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Magicube.Localization.Data {
    public class LocalizerStore : Entity<Guid> {
        public string                     CultureName { get; set; }

        [IndexField]                                         
        public string                     ModularName { get; set; }

        public Dictionary<string,string>? Localizers  { get; set; }
    }

    public class LocalizerEntityMap : EntityTypeConfiguration<LocalizerStore> {
        public override void Configure(EntityTypeBuilder<LocalizerStore> builder) {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Localizers).HasMaxLength(8000).HasConversion(x => Json.Stringify(x, null), x => x.JsonToObject<Dictionary<string, string>>());
        }
    }
}
