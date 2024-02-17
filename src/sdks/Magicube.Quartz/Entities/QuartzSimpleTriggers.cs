using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicube.Quartz {
    [Table("quartz_simple_triggers")]
	public class QuartzSimpleTriggers : IEntity {
        [Key]
        [ColumnExtend(Size = 120)]
        public string SchedName      { get; set; }

        [Key]
        [ColumnExtend(Size = 200)]
        public string TriggerName    { get; set; }

        [Key]
        [ColumnExtend(Size = 200)]
        public string TriggerGroup   { get; set; }

        public long   RepeatCount    { get; set; }
                      
        public long   RepeatInterval { get; set; }
                      
        public long   TimesTriggered { get; set; }

        public QuartzTriggers QuartzTriggers { get; set; }

        [NotMapped]
        public JObject Parts { get; set; }
    }

    public class QuartzSimpleTriggersMapping : EntityTypeConfiguration<QuartzSimpleTriggers> {
        public override void Configure(EntityTypeBuilder<QuartzSimpleTriggers> builder) {
            builder.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });

            builder.ToTable("quartz_simple_triggers");

            builder.Property(e => e.SchedName).HasColumnName("sched_name").HasMaxLength(120);

            builder.Property(e => e.TriggerName).HasColumnName("trigger_name").HasMaxLength(150);

            builder.Property(e => e.TriggerGroup).HasColumnName("trigger_group").HasMaxLength(150);

            builder.Property(e => e.RepeatCount).HasColumnName("repeat_count");

            builder.Property(e => e.RepeatInterval).HasColumnName("repeat_interval");

            builder.Property(e => e.TimesTriggered).HasColumnName("times_triggered");

            builder.HasOne(d => d.QuartzTriggers)
                .WithOne(p => p.QuartzSimpleTriggers)
                .HasForeignKey<QuartzSimpleTriggers>(d => new { d.SchedName, d.TriggerName, d.TriggerGroup })
                .HasConstraintName("FK__quartz_simple_tr__25DB9BFC");
        }
    }
}
