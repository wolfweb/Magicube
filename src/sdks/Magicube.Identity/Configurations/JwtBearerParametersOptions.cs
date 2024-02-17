using Magicube.Web.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Magicube.Identity {
    public class JwtBearerParametersOptions : AuthenticationSecrityOption {
        public string ValidIssuer      { get; set; }
        public string ValidAudience    { get; set; }
        public string IssuerSigningKey { get; set; }
        public bool   CookieEnable     { get; set; }
        public string CookieName       { get; set; }
        public string Algorithms       { get; set; } = SecurityAlgorithms.HmacSha512;
    }
}
