using System.Collections.Generic;

namespace Magicube.WebServer {
    public interface IUserIdentity {
        string UserName { get; }
        IEnumerable<string> Claims { get; }
    }

}
