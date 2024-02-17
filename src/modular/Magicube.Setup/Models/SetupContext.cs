using Magicube.Web;
using Magicube.Web.Sites;

namespace Magicube.Setup.Models {
    public class SetupContext : ISetupContext {
        public string      SupperUser { get; set; }
        public string      Password   { get; set; }
        public string      Email      { get; set; }
        public DefaultSite Site       { get; set; }
    }
}
