using Magicube.Core;
using Magicube.Core.Convertion;
using Magicube.MessageService.RabbitMQ.EndPoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.MessageService.RabbitMQ {
    public class RabbitConsumerProvider : BaseConsumerProvider {
        private readonly RabbitConsumerEndpoint _endpoint;
        private readonly IRabbitConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitConsumerProvider> _logger;
        private readonly ConcurrentDictionary<string, ConsumerRunner> ConsumerRunners = new ConcurrentDictionary<string, ConsumerRunner>();

        public RabbitConsumerProvider(
            ILogger<RabbitConsumerProvider> logger,
            IOptions<MessageOptions> options,
            RabbitMessageBuilder builder,
            Application app
            ) : base(options, app) {
            _logger            = logger;
            _endpoint          = builder.ConsumerEndpoint;
            _connectionFactory = Application.CreateScope().ServiceProvider.GetRequiredService<IRabbitConnectionFactory>();
        }

        public override async Task StartAsync(CancellationToken cancellationToken = default) {
            foreach(var consumer in MessageOptions.Consumers) {
                var (channel, queueName) = _connectionFactory.GetChannel(_endpoint);
                using (var scope = Application.CreateScope()) {
                    var _consumer = scope.ServiceProvider.Resolve<IConsumer>(consumer.ConsumerType);
                    var runner = new ConsumerRunner(queueName, _endpoint, _consumer, channel, scope);
                    ConsumerRunners.TryAdd(queueName, runner);
                    await runner.Run();
                }
            }
        }

        sealed class ConsumerRunner {
            private readonly IModel _model;
            private readonly string _queueName;
            private readonly IConsumer _consumer;
            private readonly IServiceScope _scope;
            private readonly RabbitConsumerEndpoint _endpoint;

            private AsyncEventingBasicConsumer BasicConsumer { get; set; }

            public ConsumerRunner(string queueName, RabbitConsumerEndpoint endpoint, IConsumer consumer, IModel model, IServiceScope scope) {
                _model     = model;
                _scope     = scope;
                _endpoint  = endpoint;
                _consumer  = consumer;
                _queueName = queueName;
            }
            public Task Run() {
                _model.BasicQos(0, _endpoint.CunsumerMaxBatchSize, false);
                BasicConsumer = new AsyncEventingBasicConsumer(_model);
                BasicConsumer.Received += async (model, arg) => {
                    var body = arg.Body;
                    var message = Bson.Parse<object>(body.ToArray());

                    Trace.WriteLine($"consume event message {arg.ConsumerTag} => Value: {message}");
                    MessageHeaders headers = arg.ConsumerTag;
                    headers.Add("Delivery", arg.DeliveryTag);
                    headers.Add("Exchange", arg.Exchange);
                    headers.Add("RoutingKey", arg.RoutingKey);

                    var messageBody = new MessageBody(message, headers);
                    await _consumer.ConsumeAsync(messageBody, new MessageContext { 
                        Scope = _scope
                    });
                };
                _model.BasicConsume(_queueName, _endpoint.AutoAck, BasicConsumer);
                return Task.CompletedTask;
            }
        }
    }
}
