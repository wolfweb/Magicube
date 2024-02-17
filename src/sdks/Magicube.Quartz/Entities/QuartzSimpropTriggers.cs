using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicube.Quartz {
    [Table("quartz_simprop_triggers")]
	public class QuartzSimpropTriggers : IEntity {
        [Key]
        [ColumnExtend(Size = 120)]
        public string   SchedName    { get; set; }

        [Key]
        [ColumnExtend(Size = 200)]
        public string   TriggerName  { get; set; }

        [Key]
        [ColumnExtend(Size = 200)]
        public string   TriggerGroup { get; set; }

        [ColumnExtend(Size = 512)]
        public string   StrProp1     { get; set; }

        [ColumnExtend(Size = 512)]
        public string   StrProp2     { get; set; }

        [ColumnExtend(Size = 512)]
        public string   StrProp3     { get; set; }
                        
        public int?     IntProp1     { get; set; }
                        
        public int?     IntProp2     { get; set; }
                        
        public long?    LongProp1    { get; set; }
                        
        public long?    LongProp2    { get; set; }

        public decimal? DecProp1     { get; set; }

        public decimal? DecProp2     { get; set; }

        public bool?    BoolProp1    { get; set; }
                        
        public bool?    BoolProp2    { get; set; }

        [ColumnExtend(Size = 80)]
        public string   TimeZoneId   { get; set; }

        public QuartzTriggers QuartzTriggers { get; set; }

        [NotMapped]
        public JObject Parts { get; set; }
    }

    public class QuartzSimpropTriggersMapping : EntityTypeConfiguration<QuartzSimpropTriggers> {
        public override void Configure(EntityTypeBuilder<QuartzSimpropTriggers> builder) {
            builder.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });

            builder.ToTable("quartz_simprop_triggers");

            builder.Property(e => e.SchedName).HasColumnName("sched_name").HasMaxLength(120);

            builder.Property(e => e.TriggerName).HasColumnName("trigger_name").HasMaxLength(200);

            builder.Property(e => e.TriggerGroup).HasColumnName("trigger_group").HasMaxLength(200);

            builder.Property(e => e.BoolProp1).HasColumnName("bool_prop_1");

            builder.Property(e => e.BoolProp2).HasColumnName("bool_prop_2");

            builder.Property(e => e.DecProp1).HasColumnName("dec_prop_1").HasColumnType("numeric(18, 0)");

            builder.Property(e => e.DecProp2).HasColumnName("dec_prop_2").HasColumnType("numeric(18, 0)");

            builder.Property(e => e.IntProp1).HasColumnName("int_prop_1");

            builder.Property(e => e.IntProp2).HasColumnName("int_prop_2");

            builder.Property(e => e.LongProp1).HasColumnName("long_prop_1");

            builder.Property(e => e.LongProp2).HasColumnName("long_prop_2");

            builder.Property(e => e.StrProp1).HasColumnName("str_prop_1").HasMaxLength(512);

            builder.Property(e => e.StrProp2).HasColumnName("str_prop_2").HasMaxLength(512);

            builder.Property(e => e.StrProp3).HasColumnName("str_prop_3").HasMaxLength(512);

            builder.Property(e => e.TimeZoneId).HasColumnName("time_zone_id").HasMaxLength(80);

            builder.HasOne(d => d.QuartzTriggers)
                .WithOne(p => p.QuartzSimpropTriggers)
                .HasForeignKey<QuartzSimpropTriggers>(d => new { d.SchedName, d.TriggerName, d.TriggerGroup })
                .HasConstraintName("FK__quartz_simprop_t__28B808A7");
        }
    }
}
