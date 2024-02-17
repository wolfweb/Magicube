using Magicube.Core;
using Magicube.Eventbus;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Magicube.Web.SignalR {
    public abstract class OnHubEventBase : IEvent<SignalHubContext> {
        public abstract string Name     { get; }
        public virtual  string Display  => Name.ToFriendly();
        public virtual  int    Priority => 1;
        
        public async Task OnHandlingAsync(EventContext<SignalHubContext> ctx) {
            var _ctx = ctx as SignalHubEventContext;
            if (_ctx != null) {
                await OnHandlingAsync(_ctx);
            } else
                Trace.WriteLine($"trigger {Name}");
        }

        protected abstract Task OnHandlingAsync(SignalHubEventContext ctx);
    }

    public abstract class OnHubConnection : OnHubEventBase {
        public override string Name => nameof(OnHubConnection);
    }

    public abstract class OnDisConnection : OnHubEventBase {
        public override string Name => nameof(OnDisConnection);
    }

    public abstract class OnHubCommand : OnHubEventBase {
        public override string Name => nameof(OnHubCommand);
    }
}
