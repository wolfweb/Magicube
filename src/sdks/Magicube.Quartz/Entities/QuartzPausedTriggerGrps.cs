using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicube.Quartz {
    [Table("quartz_paused_trigger_grps")]
    public class QuartzPausedTriggerGrps : IEntity {
        [Key]
        [ColumnExtend(Size = 120)]
        public string SchedName    { get; set; }

        [Key]
        [ColumnExtend(Size = 200)]
        public string TriggerGroup { get; set; }

        [NotMapped]
        public JObject Parts { get; set; }
    }

    public class QuartzPausedTriggerGrpsMapping : EntityTypeConfiguration<QuartzPausedTriggerGrps> {
        public override void Configure(EntityTypeBuilder<QuartzPausedTriggerGrps> builder) {
            builder.HasKey(e => new { e.SchedName, e.TriggerGroup });

            builder.ToTable("quartz_paused_trigger_grps");

            builder.Property(e => e.SchedName).HasColumnName("sched_name").HasMaxLength(120);

            builder.Property(e => e.TriggerGroup).HasColumnName("trigger_group").HasMaxLength(200);
        }
    }
}
