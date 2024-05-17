using Magicube.MessageService.Kafka;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Magicube.TestBase;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Moq;
using Magicube.Core;

namespace Magicube.MessageService.Test {
    public class KafkaTest {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IServiceProvider ServiceProvider;
        public KafkaTest(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
            var mockEnvironment = new Mock<IHostEnvironment>();
            ServiceProvider = new ServiceCollection()
                .AddSingleton(mockEnvironment.Object)
                .AddCore()
                .AddLogging(builder => {
                    builder.AddProvider(new XUnitLoggerProvider(testOutputHelper));
                })
                .AddSingleton(x => new AtomicCounter(0))
                .AddKafka(builder => builder.ConfigureConsume(consume => {
                    consume.GroupId          = "foo_group";
                    consume.EnableAutoCommit = true;
                    consume.BootstrapServers = "192.168.3.23:9092";
                    consume.AutoOffsetReset  = Confluent.Kafka.AutoOffsetReset.Earliest;
                }).ConfigureProduce(produce => {
                    produce.BootstrapServers = "192.168.3.23:9092";
                }))
                .AddConsumer<FooConsumer>(FooConsumer.Channel)
                .AddSingleton(_testOutputHelper)
                .BuildServiceProvider();

            ServiceProvider.GetService<Application>().ServiceProvider = ServiceProvider;
        }

        [Fact]
        public void Kafka_Producer_Test() {
            int count = 10;
            var cancel = new CancellationTokenSource();
            var counter = ServiceProvider.GetService<AtomicCounter>();

            var producer = ServiceProvider.GetService<IProduceProvider>();
            Assert.NotNull(producer);
            Assert.Equal(typeof(KafkaProducer), producer.GetType());

            var consumer = ServiceProvider.GetService<IConsumerProvider>();
            Assert.NotNull(consumer);
            Assert.Equal(typeof(KafkaConsumerProvider), consumer.GetType());
            Task.Run(() => {
                consumer.StartAsync();
            });

            for (int i = 0; i < 100; i++) {
                producer.Produce(new FooEventMessage(), new MessageHeaders {
                    ["key"] = "foo"
                });
            }

            while (!cancel.IsCancellationRequested) {
                if (counter.Current == count) {
                    cancel.Cancel();
                }
                Trace.WriteLine($"current counter=>{counter.Current}");
                Thread.Sleep(100);
            }
        }
    }
}
