using System.Threading.Tasks;

namespace Magicube.Eventbus {
    public interface IEvent<T> where T : class {
        int    Priority { get; }
        string Name     { get; }
        string Display  { get; }
        Task   OnHandlingAsync(EventContext<T> ctx);
    }
}
