using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicube.Quartz {
    [Table("quartz_fired_triggers")]
    [Index("Sched_Name", "Job_Group"                         , Name = "IDX_QRTZ_FT_JG")]
    [Index("Sched_Name", "Trigger_Group"                     , Name = "IDX_QRTZ_FT_TG")]
    [Index("Sched_Name", "Instance_Name"                     , Name = "IDX_QRTZ_FT_TRIG_INST_NAME")]
    [Index("Sched_Name", "Job_Name", "Job_Group"             , Name = "IDX_QRTZ_FT_J_G")]
    [Index("Sched_Name", "Trigger_Name", "Trigger_Group"     , Name = "IDX_QRTZ_FT_T_G")]
    [Index("Sched_Name", "Instance_Name", "Requests_Recovery", Name = "IDX_QRTZ_FT_INST_JOB_REQ_RCVRY")]
    public class QuartzFiredTriggers : IEntity {
        [Key]
        [ColumnExtend(Size = 120)]
        public string Sched_Name        { get; set; }

        [Key]
        [ColumnExtend(Size = 140)]
        public string EntryId          { get; set; }

        [ColumnExtend(Size = 200)]
        public string Trigger_Name      { get; set; }

        [ColumnExtend(Size = 200)]
        public string Trigger_Group     { get; set; }

        [ColumnExtend(Size = 200)]
        public string Instance_Name     { get; set; }

        public long   FiredTime        { get; set; }
                      
        public long   SchedTime        { get; set; }
                      
        public int    Priority         { get; set; }

        [ColumnExtend(Size = 16)]
        public string State            { get; set; }

        [ColumnExtend(Size = 200)]
        public string Job_Name          { get; set; }

        [ColumnExtend(Size = 200)]
        public string Job_Group         { get; set; }

        public bool?  IsNonconcurrent  { get; set; }
                      
        public bool?  Requests_Recovery { get; set; }

        [NotMapped]
        public JObject Parts { get; set; }
    }

    public class QuartzFiredTriggersMapping : EntityTypeConfiguration<QuartzFiredTriggers> {
        public override void Configure(EntityTypeBuilder<QuartzFiredTriggers> builder) {
            builder.HasKey(e => new { e.Sched_Name, e.EntryId });

            builder.ToTable("quartz_fired_triggers");
            builder.Property(e => e.Sched_Name).HasColumnName("sched_name").HasMaxLength(120);
            builder.Property(e => e.EntryId).HasColumnName("entry_id").HasMaxLength(140);
            builder.Property(e => e.FiredTime).HasColumnName("fired_time");
            builder.Property(e => e.Instance_Name).IsRequired().HasColumnName("instance_name").HasMaxLength(200);
            builder.Property(e => e.IsNonconcurrent).HasColumnName("is_nonconcurrent");
            builder.Property(e => e.Job_Group).HasColumnName("job_group").HasMaxLength(200);
            builder.Property(e => e.Job_Name).HasColumnName("job_name").HasMaxLength(200);
            builder.Property(e => e.Priority).HasColumnName("priority");
            builder.Property(e => e.Requests_Recovery).HasColumnName("requests_recovery");
            builder.Property(e => e.SchedTime).HasColumnName("sched_time");
            builder.Property(e => e.State).IsRequired().HasColumnName("state").HasMaxLength(16);
            builder.Property(e => e.Trigger_Group).IsRequired().HasColumnName("trigger_group").HasMaxLength(200);
            builder.Property(e => e.Trigger_Name)
                .IsRequired()
                .HasColumnName("trigger_name")
                .HasMaxLength(200);
        }
    }
}
