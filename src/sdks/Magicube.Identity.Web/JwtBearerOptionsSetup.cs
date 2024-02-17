using Magicube.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Linq;

namespace Magicube.Web {
    public class JwtBearerOptionsSetup : IPostConfigureOptions<JwtBearerOptions> {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public JwtBearerOptionsSetup(IServiceScopeFactory serviceScopeFactory) {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void PostConfigure(string name, JwtBearerOptions options) {
            options.TokenValidationParameters.AudienceValidator = (audiences, securityToken, validationParameters) => {
                var castedToken = securityToken as JsonWebToken;
                using var scoped = _serviceScopeFactory.CreateScope();
                var jwtTokenService = scoped.ServiceProvider.GetService<JwtTokenService<User>>();
                var user = castedToken.Claims.ToUser() as User;
                var ticket = castedToken.Claims.FirstOrDefault(x => x.Type == "Ticket")?.Value;
                return jwtTokenService.Validate(ticket, user);
            };
        }
    }
}
