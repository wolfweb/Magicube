using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicube.Executeflow.Entities {
    public class ActivityStore : Entity {
        public string            Name              { get; set; }
                                                   
        public Type?             Activity          { get; set; }

        [ForeignKey(IdKey)]
        public ExecuteflowStore? ExecuteflowEntity { get; set; }

        [ColumnExtend(Size = 4000)]
        public TransferContext?  Properties        { get; set; }
        public string?           Description       { get; set; }
    }

    public class ActivityEntityMapping : EntityTypeConfiguration<ActivityStore> {
        public override void Configure(EntityTypeBuilder<ActivityStore> builder) {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Properties).HasConversion(x => Json.Stringify(x, null), x => x.JsonToObject<TransferContext>());
            builder.Property(x => x.Activity).HasConversion(x => x.FullName, x => Type.GetType(x));
        }
    }
}