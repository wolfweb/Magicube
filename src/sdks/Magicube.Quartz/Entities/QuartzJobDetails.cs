using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicube.Quartz {
    [Table("quartz_job_details")]
    [Index("Sched_Name", "Requests_Recovery", Name = "IDX_QRTZ_J_GRP")]
    [Index("Sched_Name", "Job_Group", Name = "IDX_QRTZ_J_REQ_RECOVERY")]
    public class QuartzJobDetails : IEntity {
        public QuartzJobDetails() {
            QuartzTriggers = new HashSet<QuartzTriggers>();
        }

        [Key]
        [ColumnExtend(Size = 120)]
        public string Sched_Name        { get; set; }

        [Key]
        [ColumnExtend(Size = 200)]
        public string Job_Name          { get; set; }

        [Key]
        [ColumnExtend(Size = 200)]
        public string Job_Group         { get; set; }

        [ColumnExtend(Size = 250)]
        public string Description       { get; set; }

        [ColumnExtend(Size = 250)]
        public string JobClassName      { get; set; }

        public bool   IsDurable         { get; set; }
                      
        public bool   IsNonconcurrent   { get; set; }
                      
        public bool   IsUpdateData      { get; set; }
                      
        public bool   Requests_Recovery { get; set; }

        public byte[] JobData           { get; set; }

        public ICollection<QuartzTriggers> QuartzTriggers { get; set; }

        [NotMapped]
        public JObject Parts { get; set; }
    }

    public class QuartzJobDetailsMapping : EntityTypeConfiguration<QuartzJobDetails> {
        public override void Configure(EntityTypeBuilder<QuartzJobDetails> builder) {
            builder.HasKey(e => new { e.Sched_Name, e.Job_Name, e.Job_Group });
            builder.ToTable("quartz_job_details");
            builder.Property(e => e.Sched_Name).HasColumnName("sched_name").HasMaxLength(120);
            builder.Property(e => e.Job_Name).HasColumnName("job_name").HasMaxLength(200);
            builder.Property(e => e.Job_Group).HasColumnName("job_group").HasMaxLength(200);
            builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(250);
            builder.Property(e => e.IsDurable).HasColumnName("is_durable");
            builder.Property(e => e.IsNonconcurrent).HasColumnName("is_nonconcurrent");
            builder.Property(e => e.IsUpdateData).HasColumnName("is_update_data");
            builder.Property(e => e.JobClassName).IsRequired().HasColumnName("job_class_name").HasMaxLength(250);
            builder.Property(e => e.JobData).HasColumnName("job_data");
            builder.Property(e => e.Requests_Recovery).HasColumnName("requests_recovery");
        }
    }
}
