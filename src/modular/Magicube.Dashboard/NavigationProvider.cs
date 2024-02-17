using Magicube.Web.Navigation;
using Magicube.Web.Security;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace Magicube.Dashboard {
    public class NavigationProvider : INavigationProvider {
        private readonly IStringLocalizer L;

        public NavigationProvider(IStringLocalizer<NavigationProvider> localizer) {
            L = localizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder) {
            builder.Add(
                L["仪表盘"],
                design => design.Icon("fas fa-tachometer-alt").Action("Index", "Admin", new { area = "Magicube.Dashboard" }).Permission(new Permission("Magicube.Dashboard:Admin:Index")),
                null,
                -1000
                );

            return Task.CompletedTask;
        }
    }
}
