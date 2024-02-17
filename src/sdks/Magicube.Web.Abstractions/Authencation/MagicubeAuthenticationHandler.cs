using Magicube.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Magicube.Web.Authencation {
    /// <summary>
    /// authentication 用来认证用户信息
    /// </summary>    
    public class MagicubeAuthenticationHandler : AuthenticationHandler<AuthencationSchemas>{
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IAuthenticationHandlerProvider _authenticationHandlerProvider;
        public MagicubeAuthenticationHandler(
            IOptionsMonitor<AuthencationSchemas> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IAuthenticationSchemeProvider schemeProvider,
            IAuthenticationHandlerProvider authenticationHandlerProvider)
            : base(options, logger, encoder) {
            _schemeProvider                = schemeProvider;
            _authenticationHandlerProvider = authenticationHandlerProvider;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
            if (Context.User.Identity.IsAuthenticated) {
                var ticket = new AuthenticationTicket(Request.HttpContext.User, Context.User.Identity.AuthenticationType);
                return AuthenticateResult.Success(ticket);
            }
            IAuthenticationHandler authenticationHandler = null;
            string token = null;

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var cookieSchema = schemes.FirstOrDefault(x => Request.Cookies.Any(c => c.Key == x.Name));
            if(cookieSchema != null) {
                if (cookieSchema.Name != AuthencationSchemas.SchemaPrefix) {
                    authenticationHandler = await _authenticationHandlerProvider.GetHandlerAsync(Context, cookieSchema.Name);
                }
                else {
                    token = Request.Cookies[AuthencationSchemas.SchemaPrefix];
                }
            }

            if (authenticationHandler == null) {
                var authHeader = Request.Headers[HeaderNames.Authorization];
                if (!authHeader.IsNullOrEmpty()) {
                    var authHeaderValue = AuthenticationHeaderValue.Parse(authHeader);
                    var headerSchema = schemes.FirstOrDefault(x => authHeaderValue.Scheme == x.Name);

                    if (headerSchema != null) {
                        if (headerSchema.Name != AuthencationSchemas.SchemaPrefix) {
                            authenticationHandler = await _authenticationHandlerProvider.GetHandlerAsync(Context, headerSchema.Name);
                        }
                        else {
                            token = authHeaderValue.Parameter;
                        }
                    }
                }
            }

            if (authenticationHandler != null) {
                var result = await authenticationHandler.AuthenticateAsync();
                if (result.Succeeded) return result;
            }

            //特殊处理
            if (!token.IsNullOrEmpty()) { 

            }

            return AuthenticateResult.Fail("UnAuthenticated");
        }
    }
}
