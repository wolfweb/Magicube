using Magicube.Data.Abstractions.Mapping;
using Magicube.Localization.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Magicube.Users.Lang {
    public class LocalizerProvider : StorageLocalizerProvider {
        public LocalizerProvider(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor) {}

        public override string ModularName => ModularInfo.ModularName;
    }

    public class LocalizerDataEntityMap : EntityTypeConfiguration<LocalizerStore> {
        public override void Configure(EntityTypeBuilder<LocalizerStore> builder) {
            builder.HasData(new[] { 
                new LocalizerStore { 
                    CultureName = "zh-CN",
                    ModularName = ModularInfo.ModularName,
                    Localizers = new Dictionary<string, string> {
                        [""] = ""
                    }
                }
            });
        }
    }
}
