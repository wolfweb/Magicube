using Magicube.Core;
using Magicube.Web;
using Magicube.Web.Authencation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System;

namespace Magicube.Identity {
    public class MagicubeSignInManager<TUser> : SignInManager<TUser> where TUser: class, IUser {
        public MagicubeSignInManager(
            IUserClaimsPrincipalFactory<TUser> claimsFactory, 
            IOptions<IdentityOptions> optionsAccessor, 
            IHttpContextAccessor contextAccessor, 
            IAuthenticationSchemeProvider schemes, 
            IUserConfirmation<TUser> confirmation,
            ILogger<SignInManager<TUser>> logger, 
            UserManager<TUser> userManager
            ) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation) {
        }

        public override bool IsSignedIn(ClaimsPrincipal principal) {
            if (principal == null) {
                throw new ArgumentNullException(nameof(principal));
            }
            return principal?.Identities != null && principal.Identities.Any(i => i.AuthenticationType == AuthencationSchemas.CookieScheme);
        }

        public override async Task RefreshSignInAsync(TUser user) {
            var auth = await Context.AuthenticateAsync(AuthencationSchemas.CookieScheme);
            var claims = new List<Claim>();
            var authenticationMethod = auth?.Principal?.FindFirst(ClaimTypes.AuthenticationMethod);
            if (authenticationMethod != null) {
                claims.Add(authenticationMethod);
            }
            var amr = auth?.Principal?.FindFirst("amr");
            if (amr != null) {
                claims.Add(amr);
            }

            await SignInWithClaimsAsync(user, auth?.Properties, claims);
        }

        public override async Task SignInWithClaimsAsync(TUser user, AuthenticationProperties authenticationProperties, IEnumerable<Claim> additionalClaims) {
            var userPrincipal = await CreateUserPrincipalAsync(user);
            foreach (var claim in additionalClaims) {
                userPrincipal.Identities.First().AddClaim(claim);
            }
            await Context.SignInAsync(AuthencationSchemas.CookieScheme, userPrincipal, authenticationProperties ?? new AuthenticationProperties());
        }

        public override async Task SignOutAsync() {
            await Context.SignOutAsync(AuthencationSchemas.CookieScheme);
            await Context.SignOutAsync(AuthencationSchemas.ExternalScheme);
            await Context.SignOutAsync(AuthencationSchemas.TwoFactorUserIdScheme);
        }

        public override async Task<ClaimsPrincipal> CreateUserPrincipalAsync(TUser user) {
            var userId = await UserManager.GetUserIdAsync(user);
            var userName = await UserManager.GetUserNameAsync(user);
            var id = new ClaimsIdentity(AuthencationSchemas.CookieScheme, Options.ClaimsIdentity.UserNameClaimType,Options.ClaimsIdentity.RoleClaimType);
            id.AddClaim(new Claim(Options.ClaimsIdentity.UserIdClaimType, userId));
            id.AddClaim(new Claim(Options.ClaimsIdentity.UserNameClaimType, userName));
            
            if (!user.Avator.IsNullOrEmpty()) id.AddClaim(new Claim("Avator", user.Avator));

            if (UserManager.SupportsUserSecurityStamp) {
                id.AddClaim(new Claim(Options.ClaimsIdentity.SecurityStampClaimType, await UserManager.GetSecurityStampAsync(user)));
            }
            
            if (UserManager.SupportsUserClaim) {
                id.AddClaims(await UserManager.GetClaimsAsync(user));
            }
            
            if(UserManager.SupportsUserRole) {
                var roles = await UserManager.GetRolesAsync(user);
                foreach (var roleName in roles) {
                    id.AddClaim(new Claim(Options.ClaimsIdentity.RoleClaimType, roleName));
                }
            }

            return new ClaimsPrincipal(id);
        }
    }
}
