using Magicube.Core.Models;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.ViewModel;
using Magicube.Identity;
using System.ComponentModel.DataAnnotations;

namespace Magicube.Users.ViewModels {
    public class UserViewModel : EntityViewModel<User, long> {
        public UserViewModel(User user = null) : base(user) { 
        }

        [Sort(1)]
        [Display(Name = "密码")]
        public ValueObject Password { get; set; }

        [Display(Name = "创建时间")]
        [DataType(DataType.DateTime)]
        public ValueObject CreateAt { get; set; }
    }
}
