using Magicube.Data.Abstractions;
using Newtonsoft.Json.Linq;

namespace Magicube.Web.Sites {
    public class DefaultSite : IEntity {
        public string            Theme                  { get; set; }
        public string            Title                  { get; set; } = "Magicube";
        public SiteStatus        Status                 { get; set; } = SiteStatus.UnInitialized;
        public string            SupperUser             { get; set; }
        public string            Description            { get; set; }
        public string            ProviderType           { get; set; }
        public string            RequestUrlHost         { get; set; }
        public string            RequestUrlPrefix       { get; set; }
        public string            DatabaseProvider       { get; set; }
        public string            ConnectionString       { get; set; }
        public string            UserPasswordCryptoType { get; set; } = "Md5";
        public string            UserPasswordCryptoAttr { get; set; }
        public JObject           Parts                  { get; set; } = new JObject();
    }

    public enum SiteStatus {
        UnInitialized,
        Running,
        Stoped,
        Disabled
    }
}
