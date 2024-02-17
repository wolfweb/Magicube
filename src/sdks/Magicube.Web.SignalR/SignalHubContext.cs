using Microsoft.AspNetCore.SignalR;

namespace Magicube.Web.SignalR {
    public class SignalHubContext {
        public SignalHubContext(string sessionId) {
            SessionId = sessionId;
        }
        public string            SessionId { get; }
        public IHubCallerClients Clients   { get; set; }
        public string            Command   { get; set; }
    }
}
