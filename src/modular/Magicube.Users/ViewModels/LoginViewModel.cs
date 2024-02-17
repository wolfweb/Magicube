using Magicube.Data.Abstractions.ViewModel;
using Magicube.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicube.Users.ViewModels {
    public class LoginViewModel {
        public string Email      { get; set; }
        [DataType(DataType.Password)]
        public string Password   { get; set; }
        public bool   RememberMe { get; set; }
    }
}
