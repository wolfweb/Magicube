using Magicube.Web.Navigation;
using Magicube.Web.Security;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace Magicube.Users {
    public class NavigationProvider : INavigationProvider {
        private readonly IStringLocalizer L;

        public NavigationProvider(IStringLocalizer<NavigationProvider> localizer) {
            L = localizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder) {
            builder.Add(
                L["权限"],
                design => design.Icon("fa fa-key").Add(L["用户管理"], settings => settings.Icon("fa fa-user").Action("Index", "Admin", new { area = "Magicube.Users" }).Permission(new Permission("Magicube.Users:Admin:Index")), null),
                null,
                90
                );

            return Task.CompletedTask;
        }
    }
}
