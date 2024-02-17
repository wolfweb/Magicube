using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Magicube.Web.Authencation {
    public class AuthencationSchemas : AuthenticationSchemeOptions {
        internal const string SchemaPrefix              = "Magicube";
        public   const string HeaderScheme              = SchemaPrefix;
        public   const string CookieScheme              = SchemaPrefix + ".Cookie";
        public   const string ExternalScheme            = SchemaPrefix + ".External";
        public   const string TwoFactorUserIdScheme     = SchemaPrefix + ".TwoFactorUserId";
        public   const string TwoFactorRememberMeScheme = SchemaPrefix + ".TwoFactorRememberMe";
         
        public   PathString   LoginPath                 { get; set; } = "";
        public   PathString   LogoutPath                { get; set; }
        public   PathString   AccessDeniedPath          { get; set; }
    }
}
