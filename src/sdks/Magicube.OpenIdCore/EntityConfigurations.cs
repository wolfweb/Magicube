using Magicube.Core;
using Magicube.Core.Convertion;
using Magicube.Data.Abstractions.Mapping;
using Magicube.OpenIdCore.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;
using System.Collections.Immutable;

namespace Magicube.OpenIdCore {
    public class OpenIdApplicationMapping : EntityTypeConfiguration<OpenIdApplication> {
        public override void Configure(EntityTypeBuilder<OpenIdApplication> builder) {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.RedirectUris).WithOne(x => x.Application);
            builder.HasMany(x => x.PostLogoutRedirectUris).WithOne(x => x.Application);
            builder.Property(x => x.Properties).HasMaxLength(4000).HasConversion(v => Json.Stringify(v, null), v => v.JsonToObject<JObject>());
            builder.Property(x => x.Roles).HasMaxLength(2000).HasConversion(v => Json.Stringify(v, null), v => v.JsonToObject<ImmutableArray<string>>());
            builder.Property(x => x.Permissions).HasMaxLength(4000).HasConversion(v => Json.Stringify(v, null), v => v.JsonToObject<ImmutableArray<string>>());
            builder.Property(x => x.Requirements).HasMaxLength(4000).HasConversion(v => Json.Stringify(v, null), v => v.JsonToObject<ImmutableArray<string>>());
        }
    }

    public class OpenIdAuthorizationMapping: EntityTypeConfiguration<OpenIdAuthorization> {
        public override void Configure(EntityTypeBuilder<OpenIdAuthorization> builder) {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Application);
            builder.HasMany(x => x.Tokens);
            builder.Property(x => x.Scopes).HasMaxLength(2000).HasConversion(v => Json.Stringify(v, null), v => v.JsonToObject<ImmutableArray<string>>());
            builder.Property(x => x.Properties).HasMaxLength(4000).HasConversion(v => Json.Stringify(v, null), v => v.JsonToObject<JObject>());
        }
    }

    public class OpenIdScopeMapping: EntityTypeConfiguration<OpenIdScope> {
        public override void Configure(EntityTypeBuilder<OpenIdScope> builder) {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.Resources).WithOne(x => x.OpenIdScope);
            builder.HasMany(x => x.DisplayNames).WithOne(x => x.OpenIdScope);
            builder.Property(x => x.Properties).HasMaxLength(4000).HasConversion(v => Json.Stringify(v, null), v => v.JsonToObject<JObject>());
        }
    }

    public class OpenIdTokenMapping : EntityTypeConfiguration<OpenIdToken> {
        public override void Configure(EntityTypeBuilder<OpenIdToken> builder) {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Application);
            builder.HasOne(x => x.Authorization);
            builder.Property(x => x.Properties).HasMaxLength(4000).HasConversion(v => Json.Stringify(v, null), v => v.JsonToObject<JObject>());
        }
    }
}
