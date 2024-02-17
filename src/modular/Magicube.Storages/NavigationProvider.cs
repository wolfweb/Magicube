using Magicube.Web.Navigation;
using Magicube.Web.Security;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace Magicube.Storages {
    public class NavigationProvider : INavigationProvider {
        private readonly IStringLocalizer L;

        public NavigationProvider(IStringLocalizer<NavigationProvider> localizer) {
            L = localizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder) {
            builder.Add(
                L["存储配置"],
                design => design.Icon("fa fa-database").Add(L["存储管理"], settings => settings.Icon("fa fa-wrench").Action("Index", "Admin", new { area = "Magicube.Storages" }).Permission(new Permission("Magicube.Storages:Admin:Index")), null),
                null,
                120
                );

            return Task.CompletedTask;
        }
    }
}
