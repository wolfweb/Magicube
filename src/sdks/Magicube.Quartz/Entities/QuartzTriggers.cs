using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json.Linq;

namespace Magicube.Quartz {
    [Table("quartz_triggers")]
    [Index("Sched_Name", "Job_Group"                                                        , Name = "IDX_QRTZ_T_JG")]
    [Index("Sched_Name", "Calendar_Name"                                                    , Name = "IDX_QRTZ_T_C")]
    [Index("Sched_Name", "Trigger_Group"                                                    , Name = "IDX_QRTZ_T_G")]
    [Index("Sched_Name", "Trigger_State"                                                    , Name = "IDX_QRTZ_T_STATE")]
    [Index("Sched_Name", "Next_Fire_Time"                                                   , Name = "IDX_QRTZ_T_NEXT_FIRE_TIME")]
    [Index("Sched_Name", "Job_Name", "Job_Group"                                            , Name = "IDX_QRTZ_T_J")]
    [Index("Sched_Name", "Trigger_Group", "Trigger_State"                                   , Name = "IDX_QRTZ_T_N_G_STATE")]
    [Index("Sched_Name", "Trigger_State", "Next_Fire_Time"                                  , Name = "IDX_QRTZ_T_NFT_ST")]
    [Index("Sched_Name", "Misfire_Instr", "Next_Fire_Time"                                  , Name = "IDX_QRTZ_T_NFT_MISFIRE")]
    [Index("Sched_Name", "Trigger_Name", "Trigger_Group", "Trigger_State"                   , Name = "IDX_QRTZ_T_N_STATE")]
    [Index("Sched_Name", "Misfire_Instr", "Next_Fire_Time", "Trigger_State"                 , Name = "IDX_QRTZ_T_NFT_ST_MISFIRE")]
    [Index("Sched_Name", "Misfire_Instr", "Next_Fire_Time", "Trigger_Group", "Trigger_State", Name = "IDX_QRTZ_T_NFT_ST_MISFIRE_GRP")]
    public class QuartzTriggers : IEntity {
        [Key]
        [ColumnExtend(Size = 120)]
        public string    Sched_Name     { get; set; }
        
        [Key]
        [ColumnExtend(Size = 200)]
        public string    Trigger_Name   { get; set; }
        
        [Key]
        [ColumnExtend(Size = 200)]
        public string    Trigger_Group  { get; set; }

        [ColumnExtend(Size = 200)]
        public string    Job_Name       { get; set; }

        [ColumnExtend(Size = 200)]
        public string    Job_Group      { get; set; }

        public string    Description    { get; set; }

        public DateTime? Next_Fire_Time { get; set; }
        
        public DateTime? PrevFireTime   { get; set; }
        
        public int?      Priority       { get; set; }

        public string    Trigger_State  { get; set; }
        
        public string    TriggerType    { get; set; }
        
        public DateTime  StartTime      { get; set; }
        
        public DateTime? EndTime        { get; set; }

        [ColumnExtend(Size = 200)]
        public string    Calendar_Name  { get; set; }
        
        public int?      Misfire_Instr  { get; set; }
        
        public byte[]    JobData        { get; set; }

        public QuartzJobDetails      QuartzJobDetails      { get; set; }
        public QuartzBlobTriggers    QuartzBlobTriggers    { get; set; }
        public QuartzCronTriggers    QuartzCronTriggers    { get; set; }
        public QuartzSimpleTriggers  QuartzSimpleTriggers  { get; set; }
        public QuartzSimpropTriggers QuartzSimpropTriggers { get; set; }

        [NotMapped]
        public JObject Parts { get; set; }
    }

    public class QuartzTriggersMapping : EntityTypeConfiguration<QuartzTriggers> {
        public override void Configure(EntityTypeBuilder<QuartzTriggers> builder) {
            builder.HasKey(e => new { e.Sched_Name, e.Trigger_Name, e.Trigger_Group });

            builder.ToTable("quartz_triggers");
            builder.Property(e => e.Sched_Name).HasColumnName("sched_name").HasMaxLength(120);
            builder.Property(e => e.Trigger_Name).HasColumnName("trigger_name").HasMaxLength(200);
            builder.Property(e => e.Trigger_Group).HasColumnName("trigger_group").HasMaxLength(200);
            builder.Property(e => e.Calendar_Name).HasColumnName("calendar_name").HasMaxLength(200);
            builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(250);
            builder.Property(e => e.EndTime).HasConversion(new DateTimeToTicksConverter()).HasColumnName("end_time");
            builder.Property(e => e.JobData).HasColumnName("job_data");
            builder.Property(e => e.Job_Group).IsRequired().HasColumnName("job_group").HasMaxLength(200);
            builder.Property(e => e.Job_Name).IsRequired().HasColumnName("job_name").HasMaxLength(200);
            builder.Property(e => e.Misfire_Instr).HasColumnName("misfire_instr");
            builder.Property(e => e.Next_Fire_Time).HasConversion(new DateTimeToTicksConverter()).HasColumnName("next_fire_time");
            builder.Property(e => e.PrevFireTime).HasConversion(new DateTimeToTicksConverter()).HasColumnName("prev_fire_time");
            builder.Property(e => e.Priority).HasColumnName("priority");
            builder.Property(e => e.StartTime).HasConversion(new DateTimeToTicksConverter()).HasColumnName("start_time");
            builder.Property(e => e.Trigger_State).IsRequired().HasColumnName("trigger_state").HasMaxLength(16);
            builder.Property(e => e.TriggerType).IsRequired().HasColumnName("trigger_type").HasMaxLength(8);
            builder.HasOne(d => d.QuartzJobDetails)
                .WithMany(p => p.QuartzTriggers)
                .HasForeignKey(d => new { d.Sched_Name, d.Job_Name, d.Job_Group })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__quartz_triggers__22FF2F51");
        }
    }
}
