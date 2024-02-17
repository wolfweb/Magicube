using Magicube.Core.Models;
using Magicube.Eventbus;
using Magicube.Identity;
using Magicube.Web;
using Magicube.Web.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Magicube.Users.Events {
    public class OnSetupEvent : OnSetuped {
        private readonly ILogger _logger;
        private readonly UserManager<User> _userManager;
        public OnSetupEvent(ILogger<OnSetupEvent> logger, UserManager<User> userManager) {
            _logger      = logger;
            _userManager = userManager;
        }

        public override int Priority => 100;

        public override async Task OnHandlingAsync(EventContext<ISetupContext> ctx) {
            var result = await _userManager.CreateAsync(new User {
                DisplayName = "Admin",
                CreateAt    = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                UserName    = ctx.Entity.SupperUser,
                Status      = EntityStatus.Actived,
                Email       = ctx.Entity.Email,
            }, ctx.Entity.Password);
            if (!result.Succeeded) {
                _logger.LogError(string.Join("\n", result.Errors.Select(x => x.Description)));
            }
        }
    }
}
