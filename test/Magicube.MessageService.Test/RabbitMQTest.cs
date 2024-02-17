using Magicube.MessageService.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Magicube.TestBase;
using Microsoft.Extensions.Logging;
using EasyNetQ.Topology;

namespace Magicube.MessageService.Test {
    public class RabbitMQTest {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IServiceProvider ServiceProvider;
        public RabbitMQTest(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
            ServiceProvider = new ServiceCollection()
                .AddLogging(builder => {
                    builder.AddProvider(new XUnitLoggerProvider(testOutputHelper));
                })
                .AddSingleton(x => new AtomicCounter(0))
                .AddRabbitmq(builder => {
                    builder.ConfigExchange(config => {
                        config.ConfigConnection(conn => {
                            conn.Port        = 5672;
                            conn.HostName    = "192.168.3.207";
                            conn.UserName    = "admin";
                            conn.Password    = "admin";
                            conn.VirtualHost = "/";
                        }).ConfigConsumer("demo-rabbigmq", endpoint => {
                            endpoint.QueueName = FooConsumer.Channel;
                            endpoint.Exchange.ExchangeType = ExchangeType.Direct;
                        }).ConfigProducer("demo-rabbigmq", endpoint => {
                            endpoint.Exchange.ExchangeType = ExchangeType.Direct;
                        });
                    });
                })
                .AddConsumer<FooConsumer>(FooConsumer.Channel)
                .AddSingleton(_testOutputHelper)
                .BuildServiceProvider();
        }


        [Fact]
        public async void Rabbit_Message_Test() {
            int count = 10;
            var counter = ServiceProvider.GetService<AtomicCounter>();
            var cancel = new CancellationTokenSource();

            var producer = ServiceProvider.GetService<IProduceProvider>();
            Assert.NotNull(producer);
            Assert.Equal(typeof(RabbitProducer), producer.GetType());

            var consumer = ServiceProvider.GetService<IConsumerProvider>();
            Assert.NotNull(consumer);
            Assert.Equal(typeof(RabbitConsumerProvider), consumer.GetType());

            Task.Run(() => {
                consumer.StartAsync();
            });

            Task.Run(() => {
                for (int i = 0; i < count; i++) {
                    producer.Produce(new FooEventMessage(), new MessageHeaders {
                        ["endpointName"] = FooConsumer.Channel
                    });
                }
            });

            await Task.Delay(1000);

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
