using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicube.Quartz {
    [Table("quartz_calendars")]
	public class QuartzCalendars : IEntity {
        [Key]
        [ColumnExtend(Size = 120)]
        public string SchedName    { get; set; }
        [Key]
        [ColumnExtend(Size = 200)]
        public string CalendarName { get; set; }

        public byte[] Calendar     { get; set; }


        [NotMapped]
        public JObject Parts { get; set; }
    }

    public class QuartzCalendarsMapping : EntityTypeConfiguration<QuartzCalendars> {
        public override void Configure(EntityTypeBuilder<QuartzCalendars> builder) {
            builder.HasKey(e => new { e.SchedName, e.CalendarName });

            builder.ToTable("quartz_calendars");

            builder.Property(e => e.SchedName).HasColumnName("sched_name").HasMaxLength(120);

            builder.Property(e => e.CalendarName).HasColumnName("calendar_name").HasMaxLength(200);

            builder.Property(e => e.Calendar).IsRequired().HasColumnName("calendar");
        }
    }
}
