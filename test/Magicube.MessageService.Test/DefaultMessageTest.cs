using Magicube.TestBase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.Hosting;
using Moq;
using Magicube.Core;

namespace Magicube.MessageService.Test {
    public class DefaultMessageTest {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IServiceProvider ServiceProvider;

        public DefaultMessageTest(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
            var mockEnvironment = new Mock<IHostEnvironment>();

            ServiceProvider = new ServiceCollection()
                .AddSingleton(mockEnvironment.Object)
                .AddCore()
                .AddLogging(builder => {
                    builder.AddProvider(new XUnitLoggerProvider(testOutputHelper));
                })
                .AddSingleton(x => new AtomicCounter(0))
                .AddMessageCore()
                .AddConsumer<FooConsumer>(FooConsumer.Channel)
                .AddSingleton(_testOutputHelper)
                .BuildServiceProvider();

            ServiceProvider.GetService<Application>().ServiceProvider = ServiceProvider;
        }

        [Fact]
        public void Message_Core_Test() {
            int count = 10;
            var cancel = new CancellationTokenSource();
            var counter = ServiceProvider.GetService<AtomicCounter>();

            var producer = ServiceProvider.GetService<IProduceProvider>();
            Assert.NotNull(producer);
            Assert.Equal(typeof(DefaultProduceProvider), producer.GetType());

            var consumer = ServiceProvider.GetService<IConsumerProvider>();
            Assert.NotNull(consumer);
            Assert.Equal(typeof(DefaultConsumerProvider), consumer.GetType());
            Task.Run(() => {
                consumer.StartAsync();
            });

            var tokenSource = new CancellationTokenSource();

            Task.Factory.StartNew(() => {
                consumer.StartAsync(tokenSource.Token);
            }, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(() =>
            {
                Parallel.For(1, count, i => {
                    producer.Produce(Guid.NewGuid(), FooConsumer.Channel);
                });

                tokenSource.Cancel();
            }, TaskCreationOptions.LongRunning);

            while (!tokenSource.Token.IsCancellationRequested) {
                Thread.Sleep(1000);
            }
        }
    }
}
