using Magicube.Web.Navigation;
using Magicube.Web.Security;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace Magicube.Cors {
    public class NavigationProvider : INavigationProvider {
        private readonly IStringLocalizer L;

        public NavigationProvider(IStringLocalizer<NavigationProvider> localizer) {
            L = localizer;
        }
        
        public Task BuildNavigationAsync(string name, NavigationBuilder builder) {
            builder.Add(
                L["设置"],
                design => design.Icon("fa fa-cog").Add(L["跨域设置"], settings => settings.Icon("fa fa-wifi").Action("Index", "Admin", new { area = "Magicube.Cors" }).Permission(new Permission("Magicube.Cors:Admin:Index")), null, 10),
                null,
                200
                );

            return Task.CompletedTask;
        }
    }
}
