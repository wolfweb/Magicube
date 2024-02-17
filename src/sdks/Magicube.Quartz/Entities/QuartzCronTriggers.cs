using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicube.Quartz {
    [Table("quartz_cron_triggers")]
    public class QuartzCronTriggers : IEntity {
        [Key]
        [ColumnExtend(Size = 120)]
        public string SchedName      { get; set; }

        [Key]
        [ColumnExtend(Size = 200)]
        public string TriggerName    { get; set; }

        [Key]
        [ColumnExtend(Size = 200)]
        public string TriggerGroup   { get; set; }

        [ColumnExtend(Size = 120)]
        public string CronExpression { get; set; }

        [ColumnExtend(Size = 80)]
        public string TimeZoneId     { get; set; }

        public QuartzTriggers QuartzTriggers { get; set; }

        [NotMapped]
        public JObject Parts { get; set; }
    }

    public class QuartzCronTriggersMapping : EntityTypeConfiguration<QuartzCronTriggers> {
        public override void Configure(EntityTypeBuilder<QuartzCronTriggers> builder) {
            builder.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });

            builder.ToTable("quartz_cron_triggers");

            builder.Property(e => e.SchedName).HasColumnName("sched_name").HasMaxLength(120);

            builder.Property(e => e.TriggerName).HasColumnName("trigger_name").HasMaxLength(200);

            builder.Property(e => e.TriggerGroup).HasColumnName("trigger_group").HasMaxLength(200);

            builder.Property(e => e.CronExpression).IsRequired().HasColumnName("cron_expression").HasMaxLength(120);

            builder.Property(e => e.TimeZoneId).HasColumnName("time_zone_id").HasMaxLength(80);

            builder.HasOne(d => d.QuartzTriggers)
                .WithOne(p => p.QuartzCronTriggers)
                .HasForeignKey<QuartzCronTriggers>(d => new { d.SchedName, d.TriggerName, d.TriggerGroup })
                .HasConstraintName("FK__quartz_cron_trig__2B947552");
        }
    }
}
