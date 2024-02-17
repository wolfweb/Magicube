using Magicube.Core;
using Magicube.ServiceDiscovery.Abstractions;
using Microsoft.Extensions.Options;
using org.apache.zookeeper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.ServiceDiscovery.Zookeeper {
    public class ZookeeperServiceDiscovery : IServiceDiscovery {
        private const string ServiceRegistryPath = "/services";

        private readonly ZookeeperOption _options;
        private readonly IHealthCheckService _healthCheckService;
        private readonly ILoadBalancerFactory _loadBalancerFactory;
        private readonly ConcurrentDictionary<AddressEndPoint, ValueTuple<ManualResetEvent, ZooKeeper>> _zookeeperClients = new();
        public ZookeeperServiceDiscovery(IOptions<ZookeeperOption> options, IHealthCheckService healthCheckService, ILoadBalancerFactory loadBalancerFactory) {
            _options             = options.Value;
            _healthCheckService  = healthCheckService;
            _loadBalancerFactory = loadBalancerFactory;
        }

        public Task<bool> DeregisterServiceAsync(string serviceId) {
            throw new NotImplementedException();
        }

        public Task<IList<ServiceDescriptor>> FindServicesAsync(string name) {
            throw new NotImplementedException();
        }

        public Task<IList<ServiceDescriptor>> FindServicesAsync(string name, string version) {
            throw new NotImplementedException();
        }

        public Task<IList<ServiceDescriptor>> FindServicesAsync(string name, bool passingOnly = true) {
            throw new NotImplementedException();
        }

        public async Task<ServiceDescriptor> RegisterServiceAsync(string serviceName, ServiceDescriptor service, int interval = 15) {
            var client = await GetZooKeeper();
            client.Item1.WaitOne();

            string servicePath = $"{ServiceRegistryPath}/{serviceName}";

            if(await client.Item2.existsAsync(servicePath)!=null) {
                await client.Item2.createAsync(servicePath, Json.Stringify(service).ToByte(), ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);
            }
            else {
                await client.Item2.setDataAsync(servicePath, Json.Stringify(service).ToByte());
            }

            client.Item1.Reset();


            throw new NotImplementedException();
        }

        public async ValueTask<(ManualResetEvent, ZooKeeper)> GetZooKeeper() {
            var clients = await GetZooKeepers();
            return _loadBalancerFactory.Get(clients).Select();
        }

        protected virtual async ValueTask<IEnumerable<(ManualResetEvent, ZooKeeper)>> GetZooKeepers() {
            var result = new List<(ManualResetEvent, ZooKeeper)>();
            foreach (var address in _options.Addresses) {
                if (await _healthCheckService.IsHealth(address)) {
                    result.Add(CreateZooKeeper(address));
                }
            }
            return result;
        }

        protected (ManualResetEvent, ZooKeeper) CreateZooKeeper(AddressEndPoint hostPort) {
            if (!_zookeeperClients.TryGetValue(hostPort, out (ManualResetEvent, ZooKeeper) result)) {
                var connectionWait = new ManualResetEvent(false);
                result = new ValueTuple<ManualResetEvent, ZooKeeper>(connectionWait, new ZooKeeper($"{hostPort.Address}:{hostPort.Port}", (int)_options.SessionTimeout.TotalMilliseconds
                 , new ReconnectionWatcher(
                      () => {
                          connectionWait.Set();
                      },
                      () => {
                          connectionWait.Close();
                      },
                      async () => {
                          connectionWait.Reset();
                          if (_zookeeperClients.TryRemove(hostPort, out (ManualResetEvent, ZooKeeper) value)) {
                              await value.Item2.closeAsync();
                              value.Item1.Close();
                          }
                          CreateZooKeeper(hostPort);
                      })));
                _zookeeperClients.AddOrUpdate(hostPort, result, (k, v) => result);
            }
            return result;
        }

        sealed class ReconnectionWatcher : Watcher {
            private readonly Action _connectioned;
            private readonly Action _disconnect;
            private readonly Action _reconnection;

            public ReconnectionWatcher(Action connectioned, Action disconnect, Action reconnection) {
                _disconnect   = disconnect;
                _connectioned = connectioned;
                _reconnection = reconnection;
            }

            public override async Task process(WatchedEvent watchedEvent) {
                var state = watchedEvent.getState();
                switch (state) {
                    case Event.KeeperState.Expired: {
                        _reconnection();
                        break;
                    }
                    case Event.KeeperState.AuthFailed: {
                        _disconnect();
                        break;
                    }
                    case Event.KeeperState.Disconnected: {
                        _reconnection();
                        break;
                    }
                    default: {
                        _connectioned();
                        break;
                    }
                }
                await Task.CompletedTask;
            }
        }
    }
}