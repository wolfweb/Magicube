using Magicube.Core;
using Magicube.Core.Models;
using Magicube.Data.Abstractions.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Magicube.Data.Abstractions {
#nullable enable
	public class DbTable : Entity<int>, IDbTable {
		[Required]
		[Display(Name = "表单名(唯一)")]
	    [IndexField]
		public string       Name        { get; set; }

		[Display(Name = "表单显示名称")]
		public string?      Title       { get; set; }

		[ColumnExtend(Size = 2000)]
		[StringLength(2000), Display(Name = "表单描述")]
		[Sort(100)]
		public string?      Description { get; set; }

		[ReadOnly(true)]
		[Sort(10)]
		public long         CreateAt    { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
					     
		[NoUIRender]     
		public long?        UpdateAt    { get; set; }

		[NoUIRender]
		[ColumnExtend(Size = 4000)]
		public DbField[]?   Fields      { get; set; }

		[Display(Name = "表单状态")]
		public EntityStatus Status      { get; set; }
	}
#nullable disable

	public class DbTableMapping : EntityTypeConfiguration<DbTable> {
		public override void Configure(EntityTypeBuilder<DbTable> builder) {
			builder.HasKey(x => x.Id);
			builder.Property(x => x.Fields).HasConversion(x => Json.Stringify(x, null), x => x.JsonToObject<DbField[]>()).HasMaxLength(4000);
			builder.Property(x => x.Status).HasConversion(new EnumToNumberConverter<EntityStatus, int>());
		}
	}
}
