using System.Threading.Tasks;

namespace Magicube.Eventbus {
    public interface IEventProvider {
        Task OnHandlerAsync<T, TEvent>(EventContext<T> eventbusContext)
                where T : class
                where TEvent : IEvent<T>;
    }
}
