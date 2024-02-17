using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Magicube.LightApp.Abstractions {
#nullable enable
    public class LightAppUserEntity : Entity {
        public long             UserId       { get; set; }
        public LightAppType     LightAppType { get; set; }
        public string?          NickName     { get; set; }
        public LightAppUserType UserType     { get; set; }
        public string?          AvatarUrl    { get; set; }
    }

#nullable disable

    public class LightAppUserEntityMapping : EntityTypeConfiguration<LightAppUserEntity> {
        public override void Configure(EntityTypeBuilder<LightAppUserEntity> builder) {
            builder.ToTable(nameof(LightAppUserEntity));
        }
    }

    public enum LightAppUserType {
        Probation,
        Normal
    }

    public enum LightAppType {
        Wechat,
        Alipay
    }
}
