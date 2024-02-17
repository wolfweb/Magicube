using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;

namespace Magicube.Identity {
#nullable enable
    public class UserLogin : Entity<long> {
        public long     UserId              { get; set; }

        public long     CreateAt            { get; set; }
        
        public string   Ticket              { get; set; }

        [IndexField]
        public string?  ProviderKey         { get; set; }
        
        [IndexField]
        public string?  LoginProvider       { get; set; }
    }
#nullable disable
}
