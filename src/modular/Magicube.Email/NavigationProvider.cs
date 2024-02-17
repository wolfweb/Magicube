using Magicube.Web.Navigation;
using Magicube.Web.Security;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace Magicube.Email {
    public class NavigationProvider : INavigationProvider {
        private readonly IStringLocalizer L;

        public NavigationProvider(IStringLocalizer<NavigationProvider> localizer) {
            L = localizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder) {
            builder.Add(
                L["设置"],
                design => design.Icon("fa fa-cog").Add(L["邮箱设置"], settings => settings.Icon("fa fa-envelope").Action("Index", "Admin", new { area = "Magicube.Email" }).Permission(new Permission("Magicube.Email:Admin:Index")), null, 30),
                null,
                200
                );

            return Task.CompletedTask;
        }
    }
}
