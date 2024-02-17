using Magicube.Web.Navigation;
using Magicube.Web.Security;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace Magicube.Search {
    public class NavigationProvider : INavigationProvider {
        private readonly IStringLocalizer L;

        public NavigationProvider(IStringLocalizer<NavigationProvider> localizer) {
            L = localizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder) {
            builder.Add(
                L["搜索"],
                design => design.Icon("fa fa-magnifying-glass")
                    .Add(L["搜索配置"], settings => settings.Icon("fa fa-magnifying-glass-plus").Action("Index", "Admin", new { area = "Magicube.Search" }).Permission(new Permission("Magicube.Search:Admin:Index")), null, 10)
                    .Add(L["搜索管理"], settings => settings.Icon("fa fa-layer-group").Action("Index", "SearchManage", new { area = "Magicube.Search" }).Permission(new Permission("Magicube.Search:SearchManage:Index")), null, 12),
                null,
                60
                );

            return Task.CompletedTask;
        }
    }
}
