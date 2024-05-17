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
using Magicube.Core;
using Microsoft.Extensions.Hosting;
using Moq;
using Magicube.MessageService.RabbitMQ.EndPoints;

namespace Magicube.MessageService.Test {
    public class RabbitMQTest {
        const string ExchangeName = "demo-rabbigmq";
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IServiceProvider ServiceProvider;
        public RabbitMQTest(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
            var mockEnvironment = new Mock<IHostEnvironment>();
            ServiceProvider = new ServiceCollection()
                .AddSingleton(mockEnvironment.Object)
                .AddCore()
                .AddLogging(builder => {
                    builder.AddProvider(new XUnitLoggerProvider(testOutputHelper));
                })
                .AddSingleton(x => new AtomicCounter(0))
                .AddRabbitmq(builder => {
                    builder.ConfigExchange(config => {
                        config.Exchange.ExchangeType = ExchangeType.Direct;

                        config.ConfigConnection(conn => {
                            conn.Port        = 5672;
                            conn.HostName    = "localhost";
                            conn.UserName    = "guest";
                            conn.Password    = "guest";
                            conn.VirtualHost = "/";
                        }).ConfigConsumer(ExchangeName, endpoint => {
                            endpoint.QueueName = FooConsumer.Channel;
                            endpoint.RoutingKey = FooConsumer.Channel;
                        }).ConfigConsumer(ExchangeName, endpoint => {
                            endpoint.QueueName = KooConsumer.Channel;
                            endpoint.RoutingKey = KooConsumer.Channel;
                        })
                        .ConfigProducer(ExchangeName, endpoint => {
                            
                        });
                    });
                })
                .AddConsumer<FooConsumer>(FooConsumer.Channel)
                .AddConsumer<KooConsumer>(KooConsumer.Channel)
                .AddSingleton(_testOutputHelper)
                .BuildServiceProvider();

            ServiceProvider.GetService<Application>().ServiceProvider = ServiceProvider;
        }


        [Fact]
        public async void Rabbit_Message_Test() {
            int count = 10;
            int step = 1;
            var counter = ServiceProvider.GetService<AtomicCounter>();
            var cancel = new CancellationTokenSource();
            var config = ServiceProvider.GetService<RabbitMessageBuilder>();

            if(config.ProducerEndpoint is RabbitExchangeProducerEndpoint exchangeProducerEndpoint) {
                if(exchangeProducerEndpoint.Exchange.ExchangeType == ExchangeType.Fanout) {
                    step = config.ConsumerEndpoints.Count;
                }
            }

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
                    producer.RabbitProduce(new FooEventMessage(), new RabbitMessageHeaders(ExchangeName, KooConsumer.Channel));
                }
            });

            await Task.Delay(1000);

            while (!cancel.IsCancellationRequested) {
                if (counter.Current == count * step) {
                    cancel.Cancel();
                }
                Trace.WriteLine($"current counter=>{counter.Current}");
                Thread.Sleep(100);
            }
        }
    }
}
