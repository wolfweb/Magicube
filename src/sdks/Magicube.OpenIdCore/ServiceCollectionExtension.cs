using Magicube.Data.Abstractions;
using Magicube.OpenIdCore.Entities;
using Magicube.OpenIdCore.Resolvers;
using Magicube.OpenIdCore.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Magicube.OpenIdCore {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddOpenIdConnection(this IServiceCollection services, 
            Action<OpenIddictCoreBuilder> openIddictBuilder = null,
            Action<OpenIddictServerBuilder> openIddictServerBuilder = null,
            Action<OpenIddictValidationBuilder> openIddictValidationBuilder = null
            ) {
            RegisterEntities(services);

            services.AddOpenIddict()
                .AddCore(options => {
                    openIddictBuilder?.Invoke(options);
                    options.SetDefaultApplicationEntity<OpenIdApplication>()
                           .SetDefaultAuthorizationEntity<OpenIdAuthorization>()
                           .SetDefaultScopeEntity<OpenIdScope>()
                           .SetDefaultTokenEntity<OpenIdToken>()

                           .ReplaceApplicationStoreResolver<OpenIdApplicationStoreResolver>()
                           .ReplaceAuthorizationStoreResolver<OpenIdAuthorizationStoreResolver>()
                           .ReplaceScopeStoreResolver<OpenIdScopeStoreResolver>()
                           .ReplaceTokenStoreResolver<OpenIdTokenStoreResolver>()

                           .AddApplicationStore<OpenIdApplicationStore>()
                           .AddAuthorizationStore<OpenIdAuthorizationStore>()
                           .AddScopeStore<OpenIdScopeStore>()
                           .AddTokenStore<OpenIdTokenStore>();

                    options.Services.TryAddSingleton<OpenIdApplicationStoreResolver.TypeResolutionCache>();
                    options.Services.TryAddSingleton<OpenIdAuthorizationStoreResolver.TypeResolutionCache>();
                    options.Services.TryAddSingleton<OpenIdScopeStoreResolver.TypeResolutionCache>();
                    options.Services.TryAddSingleton<OpenIdTokenStoreResolver.TypeResolutionCache>();

                })
                .AddServer(options => {
                    options.SetAuthorizationEndpointUris("/connect/authorize")
                           .SetDeviceEndpointUris("/connect/device")
                           .SetLogoutEndpointUris("/connect/logout")
                           .SetTokenEndpointUris("/connect/token")
                           .SetUserinfoEndpointUris("/connect/userinfo")
                           .SetVerificationEndpointUris("/connect/verify");

                    options.AllowAuthorizationCodeFlow()
                           .AllowDeviceCodeFlow()
                           .AllowPasswordFlow()
                           .AllowRefreshTokenFlow();

                    options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles, Scopes.OpenId);

                    options.UseAspNetCore()
                           .EnableStatusCodePagesIntegration()
                           .EnableAuthorizationEndpointPassthrough()
                           .EnableLogoutEndpointPassthrough()
                           .EnableTokenEndpointPassthrough()
                           .EnableUserinfoEndpointPassthrough()
                           .EnableVerificationEndpointPassthrough()
                           .DisableTransportSecurityRequirement();

                    openIddictServerBuilder?.Invoke(options);
                })
                .AddValidation(options => {
                    options.AddAudiences("resource_server");

                    openIddictValidationBuilder?.Invoke(options);

                    options.UseLocalServer();
                    options.UseAspNetCore();
                });

            return services;
        }


        private static void RegisterEntities(IServiceCollection services) {
            services
                .AddEntity<OpenIdAuthorization, OpenIdAuthorizationMapping>()
                .AddEntity<OpenIdApplication, OpenIdApplicationMapping>()
                .AddEntity<OpenIdScope, OpenIdScopeMapping>()
                .AddEntity<OpenIdToken, OpenIdTokenMapping>()
                .AddEntity<OpenIdAppLogoutUri>()
                .AddEntity<OpenIdAppRedirectUri>()
                .AddEntity<OpenIdScopeName>()
                .AddEntity<OpenidScopeResource>();
        }
    }
}
