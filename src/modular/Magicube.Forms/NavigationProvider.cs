using Magicube.Web.Navigation;
using Magicube.Web.Security;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace Magicube.Caching {
    public class NavigationProvider : INavigationProvider {
        private readonly IStringLocalizer L;

        public NavigationProvider(IStringLocalizer<NavigationProvider> localizer) {
            L = localizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder) {
            builder.Add(
                L["表单"],
                design => design.Icon("fa fa-file-text").Add(L["表单管理"], settings => settings.Icon("fa fa-book").Action("Index", "Admin", new { area = "Magicube.Forms" }).Permission(new Permission("Magicube.Forms:Admin:Index")), null),
                null,
                20
                );

            return Task.CompletedTask;
        }
    }
}
