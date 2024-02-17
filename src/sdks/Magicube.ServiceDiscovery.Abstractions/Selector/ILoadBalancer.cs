using System.Threading.Tasks;

namespace Magicube.ServiceDiscovery.Abstractions {
    public interface ILoadBalancer<T> {
        T Select();
    }
}