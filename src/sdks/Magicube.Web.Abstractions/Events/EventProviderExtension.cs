using Magicube.Eventbus;
using System.Threading.Tasks;

namespace Magicube.Web.Events {
    public static class EventProviderExtension {
        public static Task OnSetupedAsync(this IEventProvider provider, EventContext<ISetupContext> ctx) {
            return provider.OnHandlerAsync<ISetupContext, OnSetuped>(ctx);
        }
    }

    public class OnSetuped : OnEntityEvent<ISetupContext> { }
}
