using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Core {
    public abstract class ScopedProcessor : HostedService {
        protected readonly IServiceScopeFactory ServiceScopeFactory;

        protected ScopedProcessor(IServiceScopeFactory serviceScopeFactory) : base() {
            ServiceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ProcessBackgroundTask(CancellationToken stoppingToken) {
            using (var scope = ServiceScopeFactory.CreateScope()) {
                await ProcessInScope(stoppingToken, scope.ServiceProvider);
            }
        }

        public abstract Task ProcessInScope(CancellationToken stoppingToken, IServiceProvider serviceProvider);
    }
}
