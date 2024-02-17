using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Setup.Services;
using Magicube.Setup.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Magicube.Web.Sites;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Setup {
    public class HomeController : Controller {
        private readonly IStringLocalizer L;
        private IdentityOptions _identityOptions;
        private readonly ISetupService _setupService;             
        private readonly IEnumerable<DatabaseProvider> _databaseProviders;
        public HomeController(
            IStringLocalizer<HomeController> localizer,
            ISetupService setupService,
            IEnumerable<DatabaseProvider> databaseProviders,
            IOptions<IdentityOptions> identityOptions) {
            L                  = localizer;
            _setupService      = setupService;
            _identityOptions   = identityOptions.Value;
            _databaseProviders = databaseProviders;
        }
        public IActionResult Index() {
            var model = new SetupViewModel {
                DatabaseProviders = _databaseProviders
            };

            return View(model);
        }

        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost([FromForm]SetupViewModel model) {
            if (model.Password.IsNullOrEmpty()) {
                ModelState.AddModelError(nameof(model.Password), L["密码不能是空的"]);
            }

            if (model.Password != model.PasswordConfirmation) {
                ModelState.AddModelError(nameof(model.PasswordConfirmation), L["两次输入的密码不一致"]);
            }

            if (!model.Email.IsNullOrEmpty() && !model.Email.IsEmail()) {
                ModelState.AddModelError(nameof(model.Email), L["输入的Email不正确"]);
            }

            if (!model.UserName.IsNullOrEmpty() && model.UserName.Any(c => !_identityOptions.User.AllowedUserNameCharacters.Contains(c))) {
                ModelState.AddModelError(nameof(model.UserName), L["用户名'{0}'无效,只能包含数字和字母.", model.UserName]);
            }

            if (!ModelState.IsValid) {
                return View(model);
            }

            var site = new DefaultSite {
                Title            = model.SiteName,
                SupperUser       = model.UserName,
                ConnectionString = model.ConnectionString,
                DatabaseProvider = model.DatabaseProvider,
                Description      = model.Description,
            };

            await _setupService.Execute(new Models.SetupContext {
                SupperUser = model.UserName,
                Password   = model.Password,
                Email      = model.Email,
                Site       = site,
            });

            return Redirect("~/admin");
        }
    }
}
