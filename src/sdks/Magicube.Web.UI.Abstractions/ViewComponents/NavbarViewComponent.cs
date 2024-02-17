using Magicube.Web.Navigation;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Magicube.Web.UI.ViewComponents {
    public class NavbarViewComponent : ViewComponent {
        private readonly INavigationManager _navigationManager;

        public NavbarViewComponent(INavigationManager navigationManager) {
            _navigationManager = navigationManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string name) {
            var currentMenu = await _navigationManager.GetCurrentMenuAsync(name, ViewContext);

            return View(currentMenu);
        }
    }
}