using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Core.Modular;
using Magicube.Cache.Web;
using Magicube.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace Magicube.Identity {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddIdentity(this IServiceCollection services, Action<IdentityBuilder> action = null) {
            RegisterEntities(services);

            services.AddCache();

            var builder = services
                .Configure<ModularOptions>(x=> {
                    x.ModularShareAssembly.Add(typeof(StandardPermissions).Assembly);
                })
                .AddIdentityCore<User>()
                .AddRoles<Role>()
                .AddUserStore<DefaultUserStore>()
                .AddRoleStore<DefaultRoleStore>()
                .AddSignInManager<MagicubeSignInManager<User>>()
                .AddDefaultTokenProviders();

            services.AddSingleton(typeof(IOnlineService<>), typeof(OnlineService<>));

            services.Replace<ILookupNormalizer, LookupNormalizer>();

            services.AddTransient(typeof(JwtTokenService<>));

            services.Replace(new ServiceDescriptor(typeof(IPasswordHasher<User>), typeof(DefaultPasswordHasher<User>), ServiceLifetime.Scoped));

            action?.Invoke(builder);

            services.ConfigureApplicationCookie(x => {
                x.Events.OnRedirectToLogin = context => {
                    if (context.Request.IsApiRequest() && context.Response.StatusCode == (int)HttpStatusCode.OK) {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return Task.CompletedTask;
                    }

                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
                x.Events.OnRedirectToAccessDenied = context => {
                    if (context.Request.IsApiRequest() && context.Response.StatusCode == (int)HttpStatusCode.OK) {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return Task.CompletedTask;
                    }

                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
            });

            services.AddScoped<IAuthorizationHandler, RolesPermissionsHandler>();

            return services;
        }

        private static void RegisterEntities(IServiceCollection services) {
            services
                .AddEntity<UserRole>()
                .AddEntity<User, UserMapping>()
                .AddEntity<UserClaims, UserClaimMapping>()
                .AddEntity<UserLogin, UserLoginMapping>()
                .AddEntity<UserToken, UserTokenMapping>()
                .AddEntity<Role, RoleMapping>()
                .AddEntity<RoleClaim, RoleClaimMapping>();
        }

        sealed class LookupNormalizer : ILookupNormalizer {
            [return: NotNullIfNotNull("email")]
            public string NormalizeEmail(string email) => NormalizeName(email);

            [return: NotNullIfNotNull("name")]
            public string NormalizeName(string name) => name?.Normalize();
        }
    }
}
