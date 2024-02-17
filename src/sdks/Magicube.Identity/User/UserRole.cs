using Magicube.Data.Abstractions;

namespace Magicube.Identity {
    public class UserRole : Entity<long> {
        public long UserId { get; set; }
        public int  RoleId { get; set; }
    }
}
