using Magicube.Core;
using Magicube.Core.Convertion;
using Magicube.MessageService.RabbitMQ.EndPoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.MessageService.RabbitMQ {
    public class RabbitConsumerProvider : BaseConsumerProvider {
        private readonly List<RabbitConsumerEndpoint> _endpoints;
        private readonly IRabbitConnectionFactory _connectionFactory;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, ConsumerRunner> ConsumerRunners = new ConcurrentDictionary<string, ConsumerRunner>();

        public RabbitConsumerProvider(
            ILogger<RabbitConsumerProvider> logger,
            IOptions<MessageOptions> options,
            RabbitMessageBuilder builder,
            Application app
            ) : base(options, app) {
            _logger            = logger;
            _endpoints         = builder.ConsumerEndpoints;
            _connectionFactory = Application.CreateScope().ServiceProvider.GetRequiredService<IRabbitConnectionFactory>();
        }

        protected override IConsumer MatchConsumer(MessageHeaders headers, IServiceScope scope) {

            return base.MatchConsumer(headers, scope);
        }

        public override async Task StartAsync(CancellationToken cancellationToken = default) {
            foreach(var endpoint in _endpoints) {
                try {
                    var (channel, queueName) = _connectionFactory.GetChannel(endpoint);
                    var runner = new ConsumerRunner(queueName, _logger, endpoint, channel, Application, this);
                    ConsumerRunners.TryAdd(queueName, runner);
                    await runner.Run();
                }catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }
        }

        sealed class ConsumerRunner {
            private readonly IModel _model;
            private readonly ILogger _logger;
            private readonly string _queueName;
            private readonly Application _application;
            private readonly RabbitConsumerEndpoint _endpoint;
            private readonly RabbitConsumerProvider _consumerProvider;

            private AsyncEventingBasicConsumer BasicConsumer { get; set; }

            public ConsumerRunner(string queueName, ILogger logger, RabbitConsumerEndpoint endpoint, IModel model, Application application, RabbitConsumerProvider consumerProvider) {
                _logger           = logger;
                _model            = model;
                _endpoint         = endpoint;
                _queueName        = queueName;
                _application      = application;
                _consumerProvider = consumerProvider;
            }
            public Task Run() {
                BasicConsumer = new AsyncEventingBasicConsumer(_model);
                BasicConsumer.Received += async (model, arg) => {
                    var body = arg.Body;
                    var message = Bson.Parse<object>(body.ToArray());

                    _logger.LogDebug($"consume event message {arg.ConsumerTag} => Value: {message}");
                    MessageHeaders headers = _queueName;
                    headers.Add("Delivery", arg.DeliveryTag);
                    headers.Add("Exchange", arg.Exchange);
                    headers.Add("RoutingKey", arg.RoutingKey);
                    headers.Add("ConsumerTag", arg.ConsumerTag);

                    var messageBody = new MessageBody(message, headers);
                    using (var scope = _application.CreateScope()) {
                        var _consumer = _consumerProvider.MatchConsumer(headers, scope);
                        await _consumer.ConsumeAsync(messageBody, new MessageContext {
                            Scope = scope
                        });
                    }
                };
                _model.BasicConsume(_queueName, _endpoint.AutoAck, BasicConsumer);
                return Task.CompletedTask;
            }
        }
    }
}
