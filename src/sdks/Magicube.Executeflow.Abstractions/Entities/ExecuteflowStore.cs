using Magicube.Core;
using Magicube.Core.Models;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Magicube.Executeflow.Entities {
    public class ExecuteflowStore : Entity {
        /// <summary>
        /// 执行流名称
        /// </summary>
        public string                        Name           { get; set; }
        /// <summary>
        /// 执行流状态
        /// </summary>
        public EntityStatus                  Status         { get; set; }
        /// <summary>
        /// 执行流创建时间
        /// </summary>
        public long                          CreatedAt      { get; set; }
        /// <summary>
        /// 执行流秒速
        /// </summary>
        public string?                       Description    { get; set; }
        /// <summary>
        /// 执行流活动模块
        /// </summary>
        public IEnumerable<ActivityStore>?   Activities     { get; set; }
        /// <summary>
        /// 执行流的图
        /// </summary>
        [ColumnExtend(Size = 4000)]
        public IEnumerable<Transition>?      Transitions    { get; set; }
        /// <summary>
        /// 触发事件名称
        /// </summary>
        public string?                       TriggerName    { get; set; }

        public bool                          IsSingleton    { get; set; }
    }

    public class ExecuteflowEntityMapping : EntityTypeConfiguration<ExecuteflowStore> {
        public override void Configure(EntityTypeBuilder<ExecuteflowStore> builder) {
            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.Activities).WithOne(x => x.ExecuteflowEntity);

            builder.Property(x => x.Transitions)
                .HasConversion(x => Json.Stringify(x, null), x => x.JsonToObject<IEnumerable<Transition>>())
                .HasMaxLength(4000);

            builder.Property(x => x.Status).HasConversion(new EnumToNumberConverter<EntityStatus, int>());
        }
    }

    public class Transition {
        public long              SourceActivityId      { get; set; }
        public string            SourceOutcomeName     { get; set; }
        public long              DestinationActivityId { get; set; }
    }
}