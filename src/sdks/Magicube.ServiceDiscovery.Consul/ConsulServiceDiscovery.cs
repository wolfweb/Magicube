using Consul;
using Magicube.Core;
using Magicube.ServiceDiscovery.Abstractions;
using Microsoft.Extensions.Options;
using Semver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Magicube.ServiceDiscovery.Consul {
    public class ConsulServiceDiscovery : IServiceDiscovery {
        private const string VERSION_PREFIX = "version-";

        private readonly ConsulOptions _options;
        private readonly IHealthCheckService _healthCheckService;
        private readonly ILoadBalancerFactory _loadBalancerFactory;
        private readonly ServiceDiscoveryChecker _serviceDiscoveryChecker;
        private readonly ConcurrentDictionary<AddressEndPoint, ConsulClient> _consulClients = new();

        public ConsulServiceDiscovery(
            IOptions<ConsulOptions> options,
            IHealthCheckService healthCheckService,
            ILoadBalancerFactory loadBalancerFactory,
            ServiceDiscoveryChecker serviceDiscoveryChecker) {
            _options                 = options.Value;
            _healthCheckService      = healthCheckService;
            _loadBalancerFactory     = loadBalancerFactory;
            _serviceDiscoveryChecker = serviceDiscoveryChecker;
        }

        public async Task<bool> DeregisterServiceAsync(string serviceId) {
            var _consul = await GetClient();
            var writeResult = await _consul.Agent.ServiceDeregister(serviceId);
            bool isSuccess  = writeResult.StatusCode == HttpStatusCode.OK;
            return isSuccess;
        }

        public async Task<IList<ServiceDescriptor>> FindServicesAsync(string name) {
            var _consul = await GetClient();
            var queryResult = await _consul.Health.Service(name);
            var instances = queryResult.Response.Select(serviceEntry => new ServiceDescriptor {
                Name        = serviceEntry.Service.Service,
                EndPoint    = new AddressEndPoint(serviceEntry.Service.Address, serviceEntry.Service.Port),
                Version     = GetVersionFromStrings(serviceEntry.Service.Tags),
                Id          = serviceEntry.Service.ID
            });

            return instances.ToList();
        }

        public async Task<IList<ServiceDescriptor>> FindServicesAsync(string name, bool passingOnly = true) {
            var _consul = await GetClient();
            var queryResult = await _consul.Health.Service(name, tag: "", passingOnly: passingOnly);
            var instances   = queryResult.Response.Select(serviceEntry => new ServiceDescriptor {
                Name        = serviceEntry.Service.Service,
                EndPoint    = new AddressEndPoint(serviceEntry.Service.Address, serviceEntry.Service.Port),
                Version     = GetVersionFromStrings(serviceEntry.Service.Tags),
                Id          = serviceEntry.Service.ID
            });

            return instances.ToList();
        }

        public async Task<IList<ServiceDescriptor>> FindServicesAsync(string name, string version) {
            var instances = await FindServicesAsync(name);
            var range     = SemVersionRange.Parse(version);

            return instances.Where(x => SemVersion.Parse(x.Version, SemVersionStyles.OptionalMinorPatch).Satisfies(range)).ToArray();
        }

        public async Task<ServiceDescriptor> RegisterServiceAsync(string serviceName, ServiceDescriptor service, int interval = 15) {
            var serviceId = GetServiceId(serviceName, service.EndPoint.ToUri());
            var _consul   = await GetClient();

            var port = _serviceDiscoveryChecker.Start(service.EndPoint.Address);

            var check = new AgentServiceCheck() {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),// 服务启动多久后注册
                Interval                       = TimeSpan.FromSeconds(interval),// 健康检查时间间隔，或者称为心跳间隔
                TCP                            = $"{service.EndPoint.Address}:{port}",
                Timeout                        = TimeSpan.FromSeconds(10)
            };

            string versionLabel = $"{VERSION_PREFIX}{service.Version}";

            var tagList = new List<string> {
                versionLabel
            };

            if (service.Attributes != null) {
                if (service.Attributes.ContainsKey("tags")) {
                    tagList.AddRange(service.Attributes["tags"].ToArray());
                }
            }

            var registration = new AgentServiceRegistration {
                ID      = serviceId,
                Name    = serviceName,
                Tags    = tagList.ToArray(),
                Address = service.EndPoint.Address,
                Port    = service.EndPoint.Port,
                Check   = check
            };

            await _consul.Agent.ServiceRegister(registration);

            return new ServiceDescriptor {
                Name        = registration.Name,
                Id          = registration.ID,
                EndPoint    = new AddressEndPoint(registration.Address, registration.Port),
                Version     = service.Version,
            };
        }

        public async ValueTask<ConsulClient> GetClient() {
            var clients = await GetClients();
            return _loadBalancerFactory.Get(clients).Select();
        }

        protected virtual async ValueTask<IEnumerable<ConsulClient>> GetClients() {
            var result = new List<ConsulClient>();
            foreach (var address in _options.Addresses) {
                if (await _healthCheckService.IsHealth(address)) {
                    result.Add(_consulClients.GetOrAdd(address, new ConsulClient(config => {
                        config.Address = address.ToUri();
                    }, null, h => { h.UseProxy = false; h.Proxy = null; })));

                }
            }
            return result;
        }

        private string GetServiceId(string serviceName, Uri uri) {
            return $"{serviceName}_{uri.Host.Replace(".", "_")}_{uri.Port}";
        }

        private string GetVersionFromStrings(IEnumerable<string> strings) {
            return strings?.FirstOrDefault(x => x.StartsWith(VERSION_PREFIX, StringComparison.Ordinal)).TrimStart(VERSION_PREFIX);
        }
    }
}