using Magicube.Eventbus;
using Magicube.Identity;
using Magicube.Web;
using Magicube.Web.Events;
using Magicube.Web.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Magicube.Roles.Events {
    public class OnSetupEvent : OnSetuped {
        private readonly ILogger _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public OnSetupEvent(
            ILogger<OnSetupEvent> logger,
            UserManager<User> userManager,
            RoleManager<Role> roleManager) {
            _logger      = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public override int Priority => 10;

        public override async Task OnHandlingAsync(EventContext<ISetupContext> ctx) {
            var user = await _userManager.FindByEmailAsync(ctx.Entity.Email);

            var role = new Role {
                Name        = $"Administrator",
                DisplayName = $"Administrator",
                Description = "系统超级管理员",
                Permissions = new List<string> {
                    StandardPermissions.SiteOwner.Name
                }
            };

            var result = await _roleManager.CreateAsync(role);
            await _roleManager.AddClaimAsync(role, new Claim(Permission.ClaimType, StandardPermissions.SiteOwner.Name));

            if (!result.Succeeded) {
                _logger.LogError(string.Join("\n", result.Errors.Select(x => x.Description)));
                return;
            }

            if( user.UserName == ctx.Entity.Site.SupperUser) {
                await _userManager.AddToRoleAsync(user, role.Name);
            }
        } 
    }
}
