using Magicube.Web.Sites;

namespace Magicube.Web {
    public interface ISetupContext {
        string      SupperUser { get; set; }
        string      Email      { get; set; }
        string      Password   { get; set; }
        DefaultSite Site       { get; set; }
    }
}
