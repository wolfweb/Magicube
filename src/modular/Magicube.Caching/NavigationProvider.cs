using Magicube.Web.Navigation;
using Magicube.Web.Security;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

namespace Magicube.Caching {
    public class NavigationProvider : INavigationProvider {
        private readonly IStringLocalizer L;

        public NavigationProvider(IStringLocalizer<NavigationProvider> localizer) {
            L = localizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder) {
            if (string.Equals(name, "admin", StringComparison.OrdinalIgnoreCase)) {
                builder.Add(
                    L["设置"],
                    design => design.Icon("fa fa-cog").Add(L["缓存设置"], settings => settings.Icon("fa fa-memory").Action("Index", "Admin", new { area = "Magicube.Caching" }).Permission(new Permission("Magicube.Caching:Admin:Index")), null, 20),
                    null,
                    200
                    );
            }

            return Task.CompletedTask;
        }
    }
}
