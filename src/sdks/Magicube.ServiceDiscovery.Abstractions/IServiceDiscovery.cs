using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magicube.ServiceDiscovery.Abstractions {
    public interface IServiceDiscovery {
        Task<bool> DeregisterServiceAsync(string serviceId);

        Task<IList<ServiceDescriptor>> FindServicesAsync(string name);
        Task<IList<ServiceDescriptor>> FindServicesAsync(string name, string version);
        Task<IList<ServiceDescriptor>> FindServicesAsync(string name, bool passingOnly = true);

        Task<ServiceDescriptor> RegisterServiceAsync(string serviceName, ServiceDescriptor service, int interval = 15);
    }
}