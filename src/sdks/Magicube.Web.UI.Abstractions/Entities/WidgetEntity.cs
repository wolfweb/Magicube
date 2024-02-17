using Magicube.Core.Models;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicube.Web.UI.Entities {
    public class WebWidget : Entity<int> {
        [IndexField(IsUnique = true)]
        public string       Name     { get; set; }
        [ColumnExtend(Size = 4000)]
        public string       Content  { get; set; }
        public EntityStatus Status   { get; set; }
        public long         CreateAt { get; set; }
        public long?        UpdateAt { get; set; }
    }

    public class WebLayout : Entity<int> {
        public string       Name     { get; set; }
        public string       Remark   { get; set; }
        public long         CreateAt { get; set; }
        public long?        UpdateAt { get; set; }
        [ColumnExtend(Size = 4000)]
        public string       Content  { get; set; }
        [ColumnExtend(Size = 4000)]
        public string       Schema   { get; set; }
        public EntityStatus Status   { get; set; }
    }

    public class WebPage : Entity<int> {
        public string           Name        { get; set; }
        public EntityStatus     Status      { get; set; }
        public long             CreateAt    { get; set; }
        public long?            UpdateAt    { get; set; }
        public string           Path        { get; set; }
        [ForeignKey(Entity.IdKey)]
        public WebLayout  Content     { get; set; }
        public string           Body        { get; set; }
    }

    public class WebWidgetEntityMapping : EntityTypeConfiguration<WebWidget> {
        public override void Configure(EntityTypeBuilder<WebWidget> builder) {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Content).HasMaxLength(4000);
            builder.Property(x => x.Status).HasConversion(new EnumToNumberConverter<EntityStatus, int>());
        }
    }

    public class WebPageEntityMapping : EntityTypeConfiguration<WebPage> {
        public override void Configure(EntityTypeBuilder<WebPage> builder) {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Content);
            builder.Property(x => x.Body).HasMaxLength(4000);
            builder.Property(x => x.Status).HasConversion(new EnumToNumberConverter<EntityStatus, int>());
        }
    }
    public class WebLayoutEntityMapping : EntityTypeConfiguration<WebLayout> {
        public override void Configure(EntityTypeBuilder<WebLayout> builder) {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Schema).HasMaxLength(4000);
            builder.Property(x => x.Content).HasMaxLength(4000);
            builder.Property(x => x.Status).HasConversion(new EnumToNumberConverter<EntityStatus, int>());
        }
    }    
}
