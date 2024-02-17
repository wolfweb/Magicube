using Magicube.Eventbus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Magicube.Web.SignalR {
    [Authorize]
    public class SignalHubCenter : Hub {
        private readonly ILogger _logger;
        private readonly IEventProvider _eventProvider;

        public SignalHubCenter(
            ILogger<SignalHubCenter> logger,
            IEventProvider eventProvider
            ) {
            _logger              = logger;
            _eventProvider       = eventProvider;
        }

        public override Task OnConnectedAsync() {
            string sessionId = Context.ConnectionId;
            _logger.LogDebug("SignalR已连接");

            _eventProvider.OnConnectionAsync(new SignalHubContext(sessionId){
                Clients = Clients,
            });

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception) {
            string sessionId = Context.ConnectionId;
            _logger.LogDebug("SignalR已断开连接");

            _eventProvider.OnDisConnectionAsync(new SignalHubContext(sessionId) {
                Clients = Clients,
            });

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendCommand(string command, params object[] args) {
            string sessionId = Context.ConnectionId;

            var commandContext = new SignalHubContext(sessionId) { 
                Command = command
            };
            await _eventProvider.OnCommandAsync(commandContext);
        }
    }
}
