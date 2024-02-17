using Magicube.Web.Navigation;
using Magicube.Web.Security;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace Magicube.Site {
    public class NavigationProvider : INavigationProvider {
        private readonly IStringLocalizer L;

        public NavigationProvider(IStringLocalizer<NavigationProvider> localizer) {
            L = localizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder) {
            builder.Add(
                L["设置"],
                design => design.Icon("fa fa-key").Add(L["系统设置"], settings => settings.Icon("fa fa-cog").Action("Index", "Admin", new { area = "Magicube.Site" }).Permission(new Permission("Magicube.Site:Admin:Index")), null, 100),
                null,
                200
                );

            return Task.CompletedTask;
        }
    }
}
