using Ganss.Xss;
using Magicube.Core;
using Magicube.Core.Modular;
using Magicube.Data.Abstractions;
using Magicube.Data.DbFactroy;
using Magicube.Web.ModelBinders.Polymorphic;
using Magicube.Web.Authencation;
using Magicube.Web.Environment.Builder;
using Magicube.Web.Filters;
using Magicube.Web.ModelBinders;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.WebEncoders;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using HtmlSanitizerOptions = Magicube.Web.Html.HtmlSanitizerOptions;

namespace Magicube.Web {
    public interface IWebServiceBuilder {
        void Build(WebServiceBuilder builder, IServiceProvider serviceProvider);
    }

    public class WebServiceBuilder {
        private readonly IHealthChecksBuilder HealthChecksBuilder;

        public IServiceCollection    Services              { get; private set; }
        public IMvcBuilder           MvcBuilder            { get; private set; }
        public AuthenticationBuilder AuthenticationBuilder { get; private set; }

        public WebServiceBuilder(IServiceCollection services) {
            Services = services;

            HealthChecksBuilder = services.AddHealthChecks();

            services.Configure<ModularOptions>(options => {
                options.ModularShareAssembly.AddRange(new[] {
                    typeof(IApplicationBuilder).Assembly,
                    typeof(IServiceCollection).Assembly,
                    typeof(Application).Assembly,
                    typeof(IEntity).Assembly,
                    typeof(IStartup).Assembly,
                });
            });
        }

        public WebServiceBuilder AddSecurity(
            string loginPath = "/Account/Login", 
            string logoutPath = "/Account/Logout", 
            string accessDeniedPath = "/Account/AccessDenied"
        ) {
            Services.Configure<AuthencationSchemas>(options => {
                options.LoginPath        = loginPath;
                options.LogoutPath       = logoutPath;
                options.AccessDeniedPath = accessDeniedPath;
            });

            AuthenticationBuilder = Services.AddAuthentication(AuthencationSchemas.HeaderScheme)
                .AddScheme<AuthencationSchemas, MagicubeAuthenticationHandler>(AuthencationSchemas.HeaderScheme, AuthencationSchemas.HeaderScheme, schema => { 

                })
                .AddCookie(AuthencationSchemas.CookieScheme, options => {
                    options.LoginPath        = loginPath;
                    options.AccessDeniedPath = accessDeniedPath;
                    options.Cookie.Name      = AuthencationSchemas.CookieScheme;
                    options.Cookie.HttpOnly  = true;
                });

            if (MvcBuilder == null) {
                AddMvc();
            }

            MvcBuilder.AddMvcOptions(options => {
                options.Filters.Add<AuthorizationFilter>();
                options.Filters.Add<ViewLayoutAttribute>();
            });

            return this;
        }

        public WebServiceBuilder AddMvc(Action<MvcOptions> config = null) {
            if (MvcBuilder == null) { 
                MvcBuilder = Services
                .AddControllersWithViews(option => {
                    config?.Invoke(option);
                    option.ModelBinderProviders.Insert(0, new ViewModelBinderProvider());
                    option.ModelBinderProviders.Insert(0, new ArrayModelBinderProvider());
                    option.ModelBinderProviders.Insert(0, new PolymorphicModelBinderProvider());
                })
                .AddViewLocalization()
                .AddNewtonsoftJson(options => {
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

                Services.Configure<WebEncoderOptions>(options => {
                    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
                });

                Services.AddRazorPages(options => {
                
                });

                Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

                MvcBuilder.AddMvcOptions(options => {
                    options.Filters.Add<WebApiResultFilter>();
                });
            }

            return this;
        }

        public WebServiceBuilder AddJsonOptions(Action<MvcNewtonsoftJsonOptions> config = null) {
            if (MvcBuilder == null) {
                AddMvc();
            }

            MvcBuilder.AddNewtonsoftJson(option => {
                config?.Invoke(option);
            });
            return this;
        }

        /// <summary>
        /// like *.abc.com
        /// </summary>
        /// <param name="origins"></param>
        /// <returns></returns>
        public WebServiceBuilder AddCors(params string[] origins) {
            Services.AddTransient<ICorsService, WildcardCorsService>()
                .AddTransient<ICorsPolicyProvider, CorsPolicyProvider>();

            if (origins.Any()) {
                Services.AddCors(options => {
                    options.AddDefaultPolicy(builder => {
                        builder
                        .WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    });
                });
            }

            return this;
        }

        public WebServiceBuilder AddDatabases() {
            Services.AddDatabases();
            return this;
        }

        public WebServiceBuilder ConfigureHtmlSanitizer(Action<HtmlSanitizer> action) {
            Services.Configure<HtmlSanitizerOptions>(o => {
                o.Configure.Add(action);
            });
            return this;
        }

        public IHealthChecksBuilder AddCheck<T>(string name) where T : class, IHealthCheck {
            return HealthChecksBuilder.AddCheck<T>(name);
        }

        internal IServiceCollection Build() {
            using (var serviceProvider = Services.BuildServiceProvider()) {
                var webServiceBuilders = serviceProvider.GetServices<IWebServiceBuilder>();
                foreach(var builder in webServiceBuilders) {
                    builder.Build(this, serviceProvider);
                }
            }
            return Services;
        }
    }
}
