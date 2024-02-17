using Magicube.Core.Models;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.ViewModel;
using Magicube.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Magicube.Roles.ViewModels {
    public class RoleViewModel : EntityViewModel<Role ,int> {
        public RoleViewModel(Role role = null) : base(role){ 

        }
        
        [Display(Name = "创建时间")]
        [DataType(DataType.DateTime)]
        public ValueObject         CreateAt    { get; set; }

        [NoUIRender]
        public IEnumerable<string> Permissions { get; set; }
    }
}
