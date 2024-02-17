using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicube.Executeflow.Entities {
    public class ExecuteflowStateStore : Entity { 
        public long                        UserId             { get; set; }

        public long                        CreateAt           { get; set; }

        [ForeignKey(IdKey)]
        public ExecuteflowStore?           Executeflow        { get; set; }
        
        [ColumnExtend(Size = 4000)]
        public IList<ActivityStore>?       BlockingActivities { get; set; } = new  List<ActivityStore>();

        [ColumnExtend(Size = 8000)]
        public JObject?                    State              { get; set; }

        public string?                     FaultMessage       { get; set; }
        public ExecuteflowStatus           Status             { get; set; }
    }

    public class ExecuteflowStateEntityMapping : EntityTypeConfiguration<ExecuteflowStateStore> {
        public override void Configure(EntityTypeBuilder<ExecuteflowStateStore> builder) {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Executeflow);
            builder.Property(x => x.Status).HasConversion(new EnumToNumberConverter<ExecuteflowStatus, int>());
            builder.Property(x => x.BlockingActivities).HasConversion(x => Json.Stringify(x, null), x => x.JsonToObject<IList<ActivityStore>>());
            builder.Property(x => x.State).HasConversion(x => Json.Stringify(x, null), x => x.JsonToObject<JObject>());
        }
    }
}
