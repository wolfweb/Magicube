using Magicube.Web.Navigation;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Magicube.Web.UI.ViewComponents {
    public class SideMenuViewComponent : ViewComponent {
        private readonly INavigationManager _navigationManager;

        public SideMenuViewComponent(INavigationManager navigationManager) {
            _navigationManager = navigationManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string name) {
            var AdminMenus = await _navigationManager.BuildMenuAsync(name, ViewContext);
            return View(AdminMenus);
        }
    }
}