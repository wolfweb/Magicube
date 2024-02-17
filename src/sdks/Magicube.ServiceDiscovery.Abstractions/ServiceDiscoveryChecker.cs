using System;
using System.Net.Sockets;
using System.Net;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Diagnostics;

namespace Magicube.ServiceDiscovery.Abstractions {
    public class ServiceDiscoveryChecker : IDisposable {
        private TcpListener _listener;

        private readonly ILogger _logger;
        public ServiceDiscoveryChecker(ILogger<ServiceDiscoveryChecker> logger) {
            _logger = logger;
        }

        public void Dispose() {
            _listener.Stop();
            _listener.Dispose();
            _listener = null;
        }

        public int Start(string address) {
            var ipAddr = Dns.GetHostEntry(address).AddressList.FirstOrDefault();
            _listener = new TcpListener(ipAddr, 0);
            _listener.Start();
            BeginListener();
            return ((IPEndPoint)_listener.LocalEndpoint).Port;
        }

        private async void BeginListener (){
            while (_listener != null) {
                using (var client = await _listener.AcceptTcpClientAsync()) {
                    Trace.WriteLine($"接收到客户端=>{client.Client.RemoteEndPoint}");
                    _logger.LogDebug($"接收到客户端=>{client.Client.RemoteEndPoint}");
                }
            }
        }
    }
}