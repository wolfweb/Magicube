using Magicube.Cache.Abstractions;
using Magicube.Cache.Redis;
using Magicube.Cache.Web;
using Magicube.Web.SignalR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Magicube.Caching.Events {
    public class PingRedisCommand : OnHubCommand {
        const string RedisPingKey = nameof(RedisPingKey);
        const string Command = "RedisPing";
        private readonly ICacheProvider _cacheProvider;
        public PingRedisCommand(CacheProviderFactory cacheProviderFactory) {
            _cacheProvider = cacheProviderFactory.GetCache();
        }
        protected override Task OnHandlingAsync(SignalHubEventContext ctx) {
            if (ctx.Entity.Command != Command) return Task.CompletedTask;

            _cacheProvider.Override(RedisPingKey, Guid.NewGuid().ToString("n"), TimeSpan.FromSeconds(30));

            ctx.Entity.Clients.Client(ctx.Entity.SessionId).SendAsync("RedisPong", new {
                success = _cacheProvider.Exits<string>(RedisPingKey)
            });
            return Task.CompletedTask;
        }
    }
}
