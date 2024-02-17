using Magicube.Data.Abstractions;
using System.Security.Claims;

namespace Magicube.Identity {
    public class RoleClaim : Entity<int>{
        public int    RoleId    { get; set; }

        public string ClaimType { get; set; }
        
        public string ClaimValue { get; set; }

        public Claim ToClaim() {
            return new Claim(ClaimType, ClaimValue);
        }
    }
}
