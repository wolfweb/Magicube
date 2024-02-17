using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicube.Quartz {
    [Table("quartz_locks")]
	public class QuartzLocks : IEntity {
        [Key]
        [ColumnExtend(Size = 120)]
        public string SchedName { get; set; }

        [Key]
        [ColumnExtend(Size = 40)]
        public string LockName  { get; set; }

        [NotMapped]
        public JObject Parts { get; set; }
    }

    public class QuartzLocksMapping : EntityTypeConfiguration<QuartzLocks> {
        public override void Configure(EntityTypeBuilder<QuartzLocks> builder) {
            builder.HasKey(e => new { e.SchedName, e.LockName });

            builder.ToTable("quartz_locks");

            builder.Property(e => e.SchedName).HasColumnName("sched_name").HasMaxLength(120);

            builder.Property(e => e.LockName).HasColumnName("lock_name").HasMaxLength(40);
        }
    }
}
