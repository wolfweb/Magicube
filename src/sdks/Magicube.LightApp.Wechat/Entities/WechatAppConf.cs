using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.Mapping;
using Magicube.LightApp.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Magicube.LightApp.Wechat {
#nullable enable
    public class WechatAppConf : Entity {
        public string AppId     { get; set; }
        public string AppSecret { get; set; }
    }

    public class WechatUser : LightAppUserEntity {
        public WechatUser() {
            LightAppType = LightAppType.Wechat;
        }

        [IndexField]
        public string  OpenId    { get; set; }

        public string? City      { get; set; }
        public string? Gender    { get; set; }
        public string? Country   { get; set; }
        public string? UnionId   { get; set; }
        public string? Language  { get; set; }
        public string? Province  { get; set; }
    }


    public class WechatUserMapping : EntityTypeConfiguration<WechatUser> {
        public override void Configure(EntityTypeBuilder<WechatUser> builder) {
            builder.ToTable(nameof(WechatUser));
        }
    }
#nullable disable
}
