using Magicube.Data.Abstractions;

namespace Magicube.Identity {
    public class UserClaims : Entity<long>{
        public  long   UserId     { get; set; }
        public  string ClaimType  { get; set; }
        public  string ClaimValue { get; set; }
    }
}
