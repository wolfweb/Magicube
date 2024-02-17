using System.Threading.Tasks;

namespace Magicube.Core.Environment.Eventbus {
    public interface IEventsubscriber {
        Task Invoke(IEventMessage activity);
    }
}
