using Magicube.Web.Navigation;
using Magicube.Web.Security;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace Magicube.Schedule {
    public class NavigationProvider : INavigationProvider {
        private readonly IStringLocalizer L;

        public NavigationProvider(IStringLocalizer<NavigationProvider> localizer) {
            L = localizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder) {
            builder.Add(
                L["计划任务"],
                design => design.Icon("fa fa-clock")
                    .Add(L["任务管理"], settings => settings.Icon("fa fa-stopwatch").Action("Index", "Admin", new { area = "Magicube.Schedule" }).Permission(new Permission("Magicube.Schedule:Admin:Index")), null, 10),
                null,
                60
                );

            return Task.CompletedTask;
        }
    }
}
