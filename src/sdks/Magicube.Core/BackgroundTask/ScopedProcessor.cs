using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Core {
    public abstract class ScopedProcessor : HostedService {
        protected readonly Application Application;

        protected ScopedProcessor(Application app) : base() {
            Application = app;
        }

        protected override async Task ProcessBackgroundTask(CancellationToken stoppingToken) {
            using (var scope = Application.CreateScope()) {
                await ProcessInScope(stoppingToken, scope);
            }
        }

        public abstract Task ProcessInScope(CancellationToken stoppingToken, IServiceScope scopeService);
    }
}
