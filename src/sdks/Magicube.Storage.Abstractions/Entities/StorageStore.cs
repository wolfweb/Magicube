using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Magicube.Data.Abstractions.Mapping;
using Magicube.Data.Abstractions;
using Newtonsoft.Json.Linq;
using Magicube.Data.Abstractions.ValueConversion;
using System.ComponentModel.DataAnnotations;
using Magicube.Data.Abstractions.Attributes;

namespace Magicube.Storage.Abstractions.Entities {
    public class StorageStore : Entity<int> {
        /// <summary>
        /// 名称： 用户图片， 系统图片 etc。
        /// </summary>
        [Display(Name = "存储名称")]
        public string   Name       { get; set; }
        /// <summary>
        /// 存储提供者
        /// </summary>
        [Display(Name = "存储提供者")]
        public string   Provider   { get; set; }
        /// <summary>
        /// 路径模板
        /// </summary>
        [Display(Name = "存储模板")]
        public string   Template   { get; set; }
        /// <summary>
        /// 允许上传的类型
        /// </summary>
        [Display(Name = "可上传类型")]
        [ColumnExtend(Size = 200)]
        public string[] AllowTypes { get; set; }

        /// <summary>
        /// 允许最大上传的大小
        /// </summary>
        [Display(Name = "可上传最大质量")]
        public long     MaxSize    { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ColumnExtend(Size = 2000)]
        public JObject  Attribute  { get; set; }
    }

    public class StorageStoreMapping : EntityTypeConfiguration<StorageStore> {
        public override void Configure(EntityTypeBuilder<StorageStore> builder) {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.AllowTypes).HasConversion(new JsonToStringConverter<string[]>());
            builder.Property(x => x.Attribute).HasConversion(new JsonToStringConverter<JObject>());
        }
    }
}
