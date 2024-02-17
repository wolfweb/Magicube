using Magicube.Core.Models;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicube.Identity {
#nullable enable
    public class User : Entity<long>, IUser {
        [NoUIRender]
        public int            AccessFailedCount    { get; set; }

        [NoUIRender]
        public long           CreateAt             { get; set; }
        
        [NoUIRender]
        public string?        ConcurrencyStamp     { get; set; } = Guid.NewGuid().ToString();
        
        [Display(Name = "账户")]
        [IndexField]
        public string         UserName             { get; set; }

        [Display(Name = "名称")]
        public string?        DisplayName          { get; set; }

        [Display(Name = "头像")]
        public string?        Avator               { get; set; }
        
        [Display(Name = "邮箱")]
        [IndexField(IsUnique = true)]
        public string         Email                { get; set; }

        [Display(Name = "邮箱确认状态")]
        public bool           EmailConfirmed       { get; set; }

        [Display(Name = "锁定结束时间")]
        public long?          LockoutEnd           { get; set; }

        [Display(Name = "锁定状态")]
        public bool           LockoutEnabled       { get; set; }

        [NoUIRender]
        public string         PasswordHash         { get; set; }

        [Display(Name = "电话")]
        public string?        PhoneNumber          { get; set; }

        [Display(Name = "电话确认状态")]
        public bool           PhoneNumberConfirmed { get; set; }

        [NoUIRender]
        public string?        SecurityStamp        { get; set; }

        [Display(Name = "用户状态")]
        [DataItems]
        public EntityStatus   Status               { get; set; }

        [Display(Name = "双重认证")]
        public bool           TwoFactorEnabled     { get; set; }

        public ICollection<UserRole>? UserRoles     { get; set; }
    }
#nullable disable
}
