using Magicube.Web.Navigation;
using Magicube.Web.Security;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace Magicube.Roles {
    public class NavigationProvider : INavigationProvider {
        private readonly IStringLocalizer L;

        public NavigationProvider(IStringLocalizer<NavigationProvider> localizer) {
            L = localizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder) {
            builder.Add(
                L["权限"],
                design => design.Icon("fa fa-key").Add(L["角色管理"], settings => settings.Icon("fa fa-users").Action("Index", "Admin", new { area = "Magicube.Roles" }).Permission(new Permission("Magicube.Roles:Admin:Index")), null, 10),
                null,
                80
                );

            return Task.CompletedTask;
        }
    }
}
