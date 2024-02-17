using AngleSharp.Common;
using Magicube.Cache.Web;
using Magicube.Web;
using Magicube.Web.Authencation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Magicube.Identity {
    public class JwtTokenService<TUser> where TUser : class, IUser {
        private readonly JwtBearerParametersOptions _options;

        private readonly SymmetricSecurityKey _securityKey; 
        private readonly CacheProviderFactory _cacheProviderFactory;
        private readonly MagicubeSignInManager<TUser> _signInManager;

        public JwtTokenService(
            IOptions<JwtBearerParametersOptions> jwtTokenServiceOptions, 
            CacheProviderFactory cacheProviderFactory, 
            MagicubeSignInManager<TUser> signInManager
            ) {
            _options              = jwtTokenServiceOptions.Value;
            _signInManager        = signInManager;
            _cacheProviderFactory = cacheProviderFactory;

            _securityKey          = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.IssuerSigningKey));
        }

        public async Task<(string Token, DateTime Expires)> CreateToken(TUser user, Func<IEnumerable<Claim>> handler)  {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var expires = DateTime.UtcNow.AddMinutes(_options.ExpiredTimes);

            user = await _signInManager.UserManager.FindByIdAsync(user.Id.ToString());

            var ticket  = Guid.NewGuid().ToString("n");
            var token   = jwtTokenHandler.CreateEncodedJwt(new SecurityTokenDescriptor() {
                SigningCredentials = new SigningCredentials(_securityKey, _options.Algorithms),
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim("UserName", user.UserName),
                    new Claim("Ticket", ticket)
                }.Concat(handler())),
                Expires = expires,
            });

            var logins = await _signInManager.UserManager.GetLoginsAsync(user);
            foreach (var login in logins) {
                await _signInManager.UserManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
            }

            var userInfo = new UserLoginInfo(AuthencationSchemas.HeaderScheme, ticket, user.UserName);
            await _signInManager.UserManager.AddLoginAsync(user, userInfo);

            var cacheProvider = _cacheProviderFactory.GetCache();
            cacheProvider.Override(BuildKey(user), userInfo, TimeSpan.FromMinutes(_options.ExpiredTimes));

            return (token, expires);
        }

        public TUser DecodeToken(string token, Func<IEnumerable<Claim>, TUser> convert) {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = jwtTokenHandler.ReadJwtToken(token);
            return convert(tokenDescriptor.Claims);
        }

        public bool Validate(string ticket, TUser user){
            var key = BuildKey(user);
            var cacheProvider = _cacheProviderFactory.GetCache();

            if(cacheProvider.TryGet<UserLoginInfo>(key, out var value)) {
                return value.ProviderKey == ticket;
            }
            else {
                var currentUser = _signInManager.UserManager.FindByLoginAsync(AuthencationSchemas.HeaderScheme, ticket);
                return currentUser != null;
            }
        }

        private string BuildKey(TUser user) {
            return $"UserToken:{user.Id}";
        }
    }
}
