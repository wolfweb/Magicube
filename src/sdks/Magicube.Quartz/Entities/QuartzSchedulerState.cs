using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicube.Quartz {
    [Table("quartz_scheduler_state")]
	public class QuartzSchedulerState : IEntity {
        [Key]
        [ColumnExtend(Size = 120)]
        public string SchedName       { get; set; }

        [Key]
        [ColumnExtend(Size = 200)]
        public string InstanceName    { get; set; }

        public long   LastCheckinTime { get; set; }
                      
        public long   CheckinInterval { get; set; }

        [NotMapped]
        public JObject Parts { get; set; }
    }

    public class QuartzSchedulerStateMapping : EntityTypeConfiguration<QuartzSchedulerState> {
        public override void Configure(EntityTypeBuilder<QuartzSchedulerState> builder) {
            builder.HasKey(e => new { e.SchedName, e.InstanceName });

            builder.ToTable("quartz_scheduler_state");

            builder.Property(e => e.SchedName).HasColumnName("sched_name").HasMaxLength(120);

            builder.Property(e => e.InstanceName).HasColumnName("instance_name").HasMaxLength(200);

            builder.Property(e => e.CheckinInterval).HasColumnName("checkin_interval");

            builder.Property(e => e.LastCheckinTime).HasColumnName("last_checkin_time");
        }
    }
}
