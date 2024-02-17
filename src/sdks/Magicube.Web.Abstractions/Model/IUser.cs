using Magicube.Core.Models;

namespace Magicube.Web {
    public interface IUser {
        long         Id               { get; set; }
        string       Avator           { get; set; }
        string       UserName         { get; set; }
        EntityStatus Status           { get; set; }
        string       ConcurrencyStamp { get; set; }
    }
}
