using Magicube.Eventbus;
using System.Threading.Tasks;

namespace Magicube.Web.SignalR {
    public static class EventProviderExtension {
        public static Task OnCommandAsync(this IEventProvider provider, SignalHubContext ctx) {
            return provider.OnHandlerAsync<SignalHubContext, OnHubCommand>(new SignalHubEventContext(ctx));
        }

        public static Task OnConnectionAsync(this IEventProvider provider, SignalHubContext ctx) {
            return provider.OnHandlerAsync<SignalHubContext, OnHubConnection>(new SignalHubEventContext(ctx));
        }

        public static Task OnDisConnectionAsync(this IEventProvider provider, SignalHubContext ctx) {
            return provider.OnHandlerAsync<SignalHubContext, OnDisConnection>(new SignalHubEventContext(ctx));
        }
    }
    
    public class SignalHubEventContext : EventContext<SignalHubContext> {
        public SignalHubEventContext(SignalHubContext context) : base(context) {}
    }
}
