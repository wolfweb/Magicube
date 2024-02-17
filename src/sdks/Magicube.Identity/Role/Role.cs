using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Magicube.Identity {
    public class Role : Entity<int> {
        [Display(Name = "名称")]
        [IndexField]
        public string                    Name        { get; set; }

        [Display(Name = "显示名称")]
        public string?                   DisplayName { get; set; }

        [Display(Name = "描述")]
        public string?                   Description { get; set; }

        public long                      CreateAt    { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        [QueryListFilterIgnore]
        [ColumnExtend(Size = 4000)]
        public List<string>?             Permissions { get; set; }

        [NoUIRender]
        public ICollection<UserRole>?    UserRoles { get; set; }
    }
}
