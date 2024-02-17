using Magicube.Core;
using Magicube.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Magicube.Web.SignalR {
    public class HubConnectionEvent : OnHubConnection {
        private readonly ILogger _logger;
        private readonly JwtTokenService<User> _jwtTokenService;
        private readonly IOnlineService<string> _onlineService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HubConnectionEvent(
            ILogger<HubConnectionEvent> logger,
            JwtTokenService<User> jwtTokenService,
            IOnlineService<string> onlineService,
            IHttpContextAccessor httpContextAccessor
            ) {
            _logger              = logger;
            _onlineService       = onlineService;
            _jwtTokenService     = jwtTokenService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task OnHandlingAsync(SignalHubEventContext ctx) {
            var token = _httpContextAccessor.HttpContext.Request.Query["_token"];
            if (!token.IsNullOrEmpty()) {
                var user = _jwtTokenService.DecodeToken(token, claims => claims.ToUser() as User);
                if (user != null) {
                    _onlineService.Add(new OnLineUserWrapper<string>(ctx.Entity.SessionId) { User = user });
                }
            }

            await ctx.Entity.Clients.Client(ctx.Entity.SessionId).SendAsync("OnConnected", null);
        }
    }
}
