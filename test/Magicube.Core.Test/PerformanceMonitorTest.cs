using Magicube.Core.PerformanceMonitor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Magicube.Core.Test {
    public class PerformanceMonitorTest {
        private readonly IServiceProvider ServiceProvider;
        public PerformanceMonitorTest() {
            ServiceProvider = new ServiceCollection()
                .AddCore()
                .BuildServiceProvider();
        }

        [Fact]
        public async Task Func_Performance_Test() {
            var cts = new CancellationTokenSource();
            var monitor = ServiceProvider.GetService<PerformanceMonitorFactory>();

            Parallel.For(0, 100, new ParallelOptions { MaxDegreeOfParallelism = 16 } ,i => {
                using(monitor.Start(()=> FooMethod(cts.Token))) {
                    Trace.WriteLine($"finished:{i}");
                }
            });

            using (var profile = await monitor.Start(() => FooMethod1())) {
                Trace.WriteLine($"finished result:{profile.Result}");
            }
        }

        private async Task<string> FooMethod1() {
            return await Task.FromResult(Guid.NewGuid().ToString("n"));
        }

        private void FooMethod(CancellationToken token) {
             Task.Delay(200, token).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
