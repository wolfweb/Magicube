using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicube.Quartz {
    [Table("quartz_blob_triggers")]
    public class QuartzBlobTriggers : IEntity {
        [Key]
        [ColumnExtend(Size = 120)]
        public string SchedName    { get; set; }
        [Key]
        [ColumnExtend(Size = 200)]
        public string TriggerName  { get; set; }
        [Key]
        [ColumnExtend(Size = 200)]
        public string TriggerGroup { get; set; }
        public byte[] BlobData     { get; set; }

        public QuartzTriggers QuartzTriggers { get; set; }

        [NotMapped]
        public JObject Parts { get; set; }
    }

    public class QuartzBlobTriggersMapping : EntityTypeConfiguration<QuartzBlobTriggers> {
        public override void Configure(EntityTypeBuilder<QuartzBlobTriggers> builder) {
            builder.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });

            builder.ToTable("quartz_blob_triggers");

            builder.Property(e => e.SchedName).HasColumnName("sched_name").HasMaxLength(120);

            builder.Property(e => e.TriggerName).HasColumnName("trigger_name").HasMaxLength(200);

            builder.Property(e => e.TriggerGroup).HasColumnName("trigger_group").HasMaxLength(200);

            builder.Property(e => e.BlobData).HasColumnName("blob_data");

            builder.HasOne(d => d.QuartzTriggers)
                .WithOne(p => p.QuartzBlobTriggers)
                .HasForeignKey<QuartzBlobTriggers>(d => new { d.SchedName, d.TriggerName, d.TriggerGroup })
                .HasConstraintName("FK__quartz_blob_trig");
        }
    }
}
