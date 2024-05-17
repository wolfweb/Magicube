using Magicube.Cache.Redis;
using Magicube.Core;
using Magicube.MessageService.Redis;
using Magicube.TestBase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Magicube.MessageService.Test {
    public class RedisMessageTest {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IServiceProvider ServiceProvider;
        readonly Random random = new Random();

        public RedisMessageTest(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;

            var mockEnvironment = new Mock<IHostEnvironment>();

            ServiceProvider = new ServiceCollection()
                .AddLogging(builder=> {
                    builder.AddProvider(new XUnitLoggerProvider(testOutputHelper));
                })
                .AddSingleton(x=>new AtomicCounter(0))
                .AddSingleton(mockEnvironment.Object)
                .AddCore()
                .AddRedisCache()
                .Replace<IRedisResolve, RedisResolve>()
                .AddRedisMessage()
                .AddConsumer<FooConsumer>(FooConsumer.Channel)
                .AddConsumer<KooConsumer>(KooConsumer.Channel)
                .AddSingleton(_testOutputHelper)
                .BuildServiceProvider();

            ServiceProvider.GetService<Application>().ServiceProvider = ServiceProvider;
        }

        [Fact]
        public async Task Redis_Message_Test() {
            int count = 10;
            var cancel = new CancellationTokenSource();
            var counter = ServiceProvider.GetService<AtomicCounter>();

            var producer = ServiceProvider.GetService<IProduceProvider>();
            Assert.NotNull(producer);
            Assert.Equal(typeof(RedisMessageProducer), producer.GetType());            
            
            var consumer = ServiceProvider.GetService<IConsumerProvider>();
            Assert.NotNull(consumer);
            Assert.Equal(typeof(RedisMessageConsumerProvider), consumer.GetType());

            Task.Run(() => {
                consumer.StartAsync();
            });

            await Task.Delay(1000);

            Task.Run(() => {
                for (int i = 0; i < count; i++) {
                    producer.Produce(new FooEventMessage(), new MessageHeaders {
                        ["key"] = random.Next(1,10) % 2 == 0 ?  KooConsumer.Channel : FooConsumer.Channel
                    });
                }
            });            

            while (!cancel.IsCancellationRequested) {
                if (counter.Current == count ) {
                    cancel.Cancel();
                }
                Trace.WriteLine($"current counter=>{counter.Current}");
                Thread.Sleep(100);
            }
        }

        sealed class RedisResolve : IRedisResolve {
            public IConnectionMultiplexer GetConnectionMultiplexer() {
                return ConnectionMultiplexer.Connect("localhost:6379");
            }
        }
    }
}
