using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.ServiceDiscovery.Abstractions {
    public interface IHealthCheckService {
        void Monitor(AddressEndPoint address);
        ValueTask<bool> IsHealth(AddressEndPoint address);
    }

    public class DefaultHealthCheckService : IHealthCheckService, IDisposable {
        private readonly int _timeout = 30000;
        private readonly Timer _timer;
        private readonly ConcurrentDictionary<ValueTuple<string, int>, MonitorEntry> _dictionary = new();

        public DefaultHealthCheckService() {
            var timeSpan = TimeSpan.FromSeconds(60);

            _timer = new Timer(async s =>
            {
                await Check(_dictionary.ToArray().Select(i => i.Value), _timeout);
            }, null, timeSpan, timeSpan);
        }

        public async ValueTask<bool> IsHealth(AddressEndPoint address) {
            MonitorEntry entry;
            var isHealth = !_dictionary.TryGetValue(new ValueTuple<string, int>(address.Address, address.Port), out entry) ? await Check(address, _timeout) : entry.Health;
            return isHealth;
        }

        public void Monitor(AddressEndPoint address) {
            _dictionary.GetOrAdd(new ValueTuple<string, int>(address.Address, address.Port), k => new MonitorEntry(address));
        }

        public void Dispose() {
            _timer.Dispose();
        }

        private static async Task<bool> Check(AddressEndPoint address, int timeout) {
            bool isHealth = false;
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { SendTimeout = timeout }) {
                try {
                    await socket.ConnectAsync(address);
                    isHealth = true;
                }
                catch {

                }
                return isHealth;
            }
        }

        private static async Task Check(IEnumerable<MonitorEntry> entrys, int timeout) {
            foreach (var entry in entrys) {
                using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { SendTimeout = timeout }) {
                    try {
                        await socket.ConnectAsync(entry.EndPoint);
                        entry.UnhealthyTimes = 0;
                        entry.Health = true;
                    }
                    catch {
                        entry.UnhealthyTimes++;
                        entry.Health = false;
                    }
                }
            }
        }

        protected class MonitorEntry {
            public MonitorEntry(AddressEndPoint addressModel, bool health = true) {
                EndPoint = addressModel;
                Health   = health;
            }

            public int UnhealthyTimes { get; set; }
            public EndPoint  EndPoint { get; set; }
            public bool      Health   { get; set; }
        }
    }
}