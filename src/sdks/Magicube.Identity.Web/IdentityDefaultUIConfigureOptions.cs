using Magicube.Identity.Web.Filters;
using Magicube.Web.Authencation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace Magicube.Identity.Web {
    public class IdentityDefaultUIConfigureOptions : IPostConfigureOptions<RazorPagesOptions>, IConfigureNamedOptions<CookieAuthenticationOptions> {
        private const string IdentityUIDefaultAreaName = "Identity";
        public IWebHostEnvironment Environment { get; }
        public IdentityDefaultUIConfigureOptions(IWebHostEnvironment environment) {
            Environment = environment;
        }
        public void Configure(string name, CookieAuthenticationOptions options) {
            name    = name    ?? throw new ArgumentNullException(nameof(name));
            options = options ?? throw new ArgumentNullException(nameof(options));

            if (string.Equals(AuthencationSchemas.CookieScheme, name, StringComparison.Ordinal)) {
                options.LoginPath        = "/Account/Login";
                options.LogoutPath       = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
            }
        }

        public void Configure(CookieAuthenticationOptions options) {

        }

        public void PostConfigure(string name, RazorPagesOptions options) {
            name    = name    ?? throw new ArgumentNullException(nameof(name));
            options = options ?? throw new ArgumentNullException(nameof(options));

            options.Conventions.Add(new IdentityPageRouteModelConvention());

            options.Conventions.AuthorizeAreaFolder(IdentityUIDefaultAreaName, "/Account/Manage");
            options.Conventions.AuthorizeAreaPage(IdentityUIDefaultAreaName, "/Account/Logout");

            var convention = new IdentityPageModelConvention();
            options.Conventions.AddAreaFolderApplicationModelConvention(
                IdentityUIDefaultAreaName,
                "/",
                pam => convention.Apply(pam));
            options.Conventions.AddAreaFolderApplicationModelConvention(
                IdentityUIDefaultAreaName,
                "/Account/Manage",
                pam => pam.Filters.Add(new ExternalLoginsPageFilter()));
        }
    }

    public class IdentityPageRouteModelConvention : IPageRouteModelConvention {
        public void Apply(PageRouteModel model) {
            foreach (var selector in model.Selectors.ToList()) {
                var template = selector.AttributeRouteModel.Template;
                if (template.Contains("/")) {
                    var segments = template.Split(new[] { '/' }, StringSplitOptions.None);
                    selector.AttributeRouteModel.Template = string.Join("/", segments.Skip(1));
                }
            }
        }
    }
}
