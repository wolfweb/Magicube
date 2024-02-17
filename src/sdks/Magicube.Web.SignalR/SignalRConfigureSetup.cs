using Magicube.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace Magicube.Web.SignalR {
    class SignalRConfigureSetup : IConfigureOptions<HubOptions> {
        private readonly Application _application;
        public SignalRConfigureSetup(Application application) {
            _application = application;
        }

        public void Configure(HubOptions options) {
            options.EnableDetailedErrors = _application.IsDevelop;
        }
    }
}
