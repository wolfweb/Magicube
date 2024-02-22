using Magicube.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magicube.Eventbus {
    public class EventProvider : IEventProvider {
        private readonly Application _app;

        public EventProvider(Application app) {
            _app = app;
        }

        public int Priority => 1;

        public async Task OnHandlerAsync<T, TEvent>(EventContext<T> ctx)
            where T : class
            where TEvent : IEvent<T> {
            using(var scope = _app.CreateScope()) {
                IEnumerable<IEvent<T>> events = scope.ServiceProvider.GetServices<IEvent<T>>(typeof(T).Name);
                foreach (var @event in events.OfType<TEvent>().OrderByDescending(x => x.Priority)) {
                    if (ctx.Status.IsCancellationRequested) break;

                    await @event.OnHandlingAsync(ctx);
                }
            }
        }
    }
}
