using Magicube.Core.Models;
using Magicube.Eventbus;
using Magicube.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace Magicube.LightApp.Abstractions {
    public class LightAppUserCreatingEvent : OnCreating<LightAppUserEntity> {
        private readonly UserManager<User> _userManager;
        public LightAppUserCreatingEvent(UserManager<User> userManager) {
            _userManager = userManager;
        }

        public override async Task OnHandlingAsync(EventContext<LightAppUserEntity> ctx) {
            var user = new User {
                Email            = "empty@empty.com",
                Status           = EntityStatus.Pending,
                CreateAt         = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                UserName         = "anonymous",
                DisplayName      = "匿名用户",
                PasswordHash     = DateTimeOffset.Now.ToString("yyyyMMddHHmmss"),
                SecurityStamp    = Guid.NewGuid().ToString("n"),
            };
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded) {
                ctx.Entity.UserId = user.Id;
            }
        }
    }

    public class LightAppUserUpdatingEvent : OnUpdating<LightAppUserEntity> {
        private readonly UserManager<User> _userManager;

        public LightAppUserUpdatingEvent(UserManager<User> userManager) {
            _userManager = userManager;
        }

        public override async Task OnHandlingAsync(EventContext<LightAppUserEntity> ctx) {
            var user = await _userManager.FindByIdAsync(ctx.Entity.UserId.ToString());
            user.Avator      = ctx.Entity.AvatarUrl;
            user.DisplayName = ctx.Entity.NickName;

            await _userManager.UpdateAsync(user);
        }
    }
}
