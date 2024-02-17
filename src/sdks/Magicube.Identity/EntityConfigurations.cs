using Magicube.Core;
using Magicube.Core.Models;
using Magicube.Core.Convertion;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Magicube.Identity {
    public class UserMapping : EntityTypeConfiguration<User> {
        public override void Configure(EntityTypeBuilder<User> builder) {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.PhoneNumber).HasMaxLength(32);
            builder.Property(x => x.DisplayName).HasMaxLength(128);
            builder.Property(x => x.SecurityStamp).HasMaxLength(64);
            builder.Property(x => x.ConcurrencyStamp).HasMaxLength(64);
            builder.Property(x => x.Email).HasMaxLength(128).IsRequired();
            builder.Property(x => x.UserName).HasMaxLength(128).IsRequired();
            builder.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();

            builder.Property(x => x.Status).HasConversion(new EnumToNumberConverter<EntityStatus, int>());

            builder.HasIndex(x => x.Email).HasDatabaseName("UserEmailIndex").IsUnique();
            builder.HasIndex(x => x.UserName).HasDatabaseName("UserNameIndex").IsUnique();

            builder.HasMany(e => e.UserRoles).WithOne().HasForeignKey(ur => ur.UserId);
        }
    }

    public class UserClaimMapping : EntityTypeConfiguration<UserClaims> {
        public override void Configure(EntityTypeBuilder<UserClaims> builder) {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ClaimType).HasMaxLength(128).IsRequired();
            builder.Property(x => x.ClaimValue).HasMaxLength(128).IsRequired();
        }
    }

    public class UserLoginMapping : EntityTypeConfiguration<UserLogin> {
        public override void Configure(EntityTypeBuilder<UserLogin> builder) {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ProviderKey).HasMaxLength(128).IsRequired();
            builder.Property(x => x.LoginProvider).HasMaxLength(128).IsRequired();
        }
    }

    public class UserTokenMapping : EntityTypeConfiguration<UserToken> {
        public override void Configure(EntityTypeBuilder<UserToken> builder) {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(128).IsRequired();
            builder.Property(x => x.Value).HasMaxLength(256).IsRequired();
            builder.Property(x => x.LoginProvider).HasMaxLength(128).IsRequired();
        }
    }

    public class UserRoleMapping : EntityTypeConfiguration<UserRole> {
        public override void Configure(EntityTypeBuilder<UserRole> builder) {
            builder.HasKey(x => x.Id);
        }
    }

    public class RoleMapping: EntityTypeConfiguration<Role> {
        public override void Configure(EntityTypeBuilder<Role> builder) {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(128).IsRequired();
            builder.Property(x => x.DisplayName).HasMaxLength(128).IsRequired();
            builder.Property(x => x.Permissions).HasConversion(x => Json.Stringify(x, null), x => x.JsonToObject<List<string>>());

            builder.HasMany(e => e.UserRoles)
                .WithOne()
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
            //todo: default data
            //builder.HasData()
        }
    }

    public class RoleClaimMapping: EntityTypeConfiguration<RoleClaim> {
        public override void Configure(EntityTypeBuilder<RoleClaim> builder) {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ClaimType).HasMaxLength(128).IsRequired();
            builder.Property(x => x.ClaimValue).HasMaxLength(128).IsRequired();
        }
    }
}
