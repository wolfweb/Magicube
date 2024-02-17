using Magicube.Core.Modular;
using Magicube.Core.Reflection;
using Magicube.Identity;
using Magicube.Identity.Web;
using Magicube.Net;
using Magicube.Net.Email;
using Magicube.Web.Authencation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Magicube.Web {
    public static class WebServiceBuilderExtension {
        public static WebServiceBuilder AddIdentityWeb(this WebServiceBuilder builder, Action<IdentityWebBuilder> identityWebBuilderConfig = null) {
            builder.Services
                .ConfigureOptions(typeof(IdentityDefaultUIConfigureOptions))
                .AddEmailServices(options => { 

                })
                .AddIdentity();

            if(builder.AuthenticationBuilder == null) {
                builder.AddSecurity();
            }

            var identityWebBuilder = new IdentityWebBuilder(builder);

            builder.AuthenticationBuilder
                .AddCookie(AuthencationSchemas.ExternalScheme, options => {
                    options.Cookie.Name    = AuthencationSchemas.ExternalScheme;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                }).AddCookie(AuthencationSchemas.TwoFactorRememberMeScheme, options => {
                    options.Cookie.Name = AuthencationSchemas.TwoFactorRememberMeScheme;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                    options.Events      = new CookieAuthenticationEvents {
                        OnValidatePrincipal = SecurityStampValidator.ValidateAsync<ITwoFactorSecurityStampValidator>
                    };
                }).AddCookie(AuthencationSchemas.TwoFactorUserIdScheme, options => {
                    options.Cookie.Name    = AuthencationSchemas.TwoFactorUserIdScheme;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                });

            builder.Services.Configure<ModularOptions>(options => {
                options.ModularShareAssembly.Add(typeof(IdentityDefaultUIConfigureOptions).Assembly);
                options.ModularShareAssembly.Add(typeof(MailOption).Assembly);
            });

            identityWebBuilderConfig?.Invoke(identityWebBuilder);

            return builder;
        }
    }

    public class IdentityWebBuilder {
        private readonly WebServiceBuilder _webServiceBuilder;
        public IdentityWebBuilder(WebServiceBuilder webServiceBuilder) {
            _webServiceBuilder = webServiceBuilder;
        }

        public IdentityWebBuilder Configure(Action<IdentityOptions> config = null) {
            _webServiceBuilder.Services.Configure(config);
            return this;
        }

        public IdentityWebBuilder JwtEnable(JwtBearerParametersOptions parameter) {
            _webServiceBuilder.Services.Configure<JwtBearerParametersOptions>(x => {
                TypeAccessor.Get(parameter).Copy(x);
            });

            _webServiceBuilder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

            _webServiceBuilder.AuthenticationBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateActor    = false,
                    ValidateIssuer   = false,
                    ValidateAudience = true,

                    ClockSkew        = TimeSpan.Zero,

                    ValidIssuer      = parameter.ValidIssuer,
                    ValidAudience    = parameter.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(parameter.IssuerSigningKey)),
                };

                if (parameter.CookieEnable && !string.IsNullOrEmpty(parameter.CookieName)) {
                    options.Events = new JwtBearerEvents {
                        OnMessageReceived = (ctx) => {
                            if (ctx.Request.Cookies.TryGetValue(parameter.CookieName, out var v)) {
                                ctx.Token = v;
                            }
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = async (ctx) => {
                            //定义刷新token，待测试
                            if(ctx.SecurityToken.ValidTo.Subtract(DateTime.UtcNow)< TimeSpan.FromMinutes(5)) {
                                var jwtToken = ctx.SecurityToken as JsonWebToken;
                                var user     = jwtToken.Claims.ToUser() as User;
                                var service  = ctx.HttpContext.RequestServices.GetService<JwtTokenService<User>>();
                                var token    = await service.CreateToken(user, () => Enumerable.Empty<Claim>());
                                ctx.HttpContext.Response.Headers.Append("Set-Token", token.Token);
                            }
                        }
                    };
                }
            });

            return this;
        }
    }
}
