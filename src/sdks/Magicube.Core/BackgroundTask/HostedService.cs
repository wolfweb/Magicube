using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Core {
    public abstract class HostedService : IHostedService {
        private Task _executingTask;
        private CancellationTokenSource _cts;

        public Task StartAsync(CancellationToken cancellationToken) {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _executingTask = ExecuteAsync(_cts.Token);

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken) {
            if (_executingTask == null) {
                return;
            }

            _cts.Cancel();

            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));
            cancellationToken.ThrowIfCancellationRequested();
        }

        protected virtual async Task ExecuteAsync(CancellationToken stoppingToken) {
            do {
                await ProcessBackgroundTask(stoppingToken);

                await Task.Delay(5000, stoppingToken);
            } while (!stoppingToken.IsCancellationRequested);
        }

        protected abstract Task ProcessBackgroundTask(CancellationToken stoppingToken);
    }
}
