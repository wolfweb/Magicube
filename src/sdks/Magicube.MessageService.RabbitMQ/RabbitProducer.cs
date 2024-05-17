using Magicube.Core.Convertion;
using Magicube.MessageService.RabbitMQ.EndPoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magicube.MessageService.RabbitMQ {
    public class RabbitProducer : IProduceProvider {
        private readonly ILogger<RabbitProducer> _logger;
        private readonly RabbitProducerEndpoint _endpoint;
        private readonly IRabbitConnectionFactory _connectionFactory;
        private readonly Dictionary<string, IModel> _rabbitChannels = new();

        public RabbitProducer(
            ILogger<RabbitProducer> logger,
            RabbitMessageBuilder builder,
            IServiceProvider serviceProvider 
            ) {
            _logger    = logger;
            _endpoint  = builder.ProducerEndpoint;
            _connectionFactory = serviceProvider.GetRequiredService<IRabbitConnectionFactory>();
        }

        public void Produce(object value, MessageHeaders headers) {
            var route        = headers.TryGet<string>(MessageHeaders.MessageHeaderKey);
            var endpointName = headers.TryGet<string>(RabbitMessageHeaders.ExchangeKey);

            if (!_rabbitChannels.TryGetValue(endpointName, out var model)) {
                model = _connectionFactory.GetChannel(_endpoint, endpointName);
                _rabbitChannels[endpointName] = model;
            }

            var properties = model.CreateBasicProperties();
            properties.Persistent = true;

            string routingKey;
            switch (_endpoint) {
                case RabbitQueueProducerEndpoint queueEndpoint:
                    routingKey = queueEndpoint.Name;
                    model.BasicPublish(
                        string.Empty,
                        routingKey,
                        properties,
                        Bson.Serialize(value));
                    break;
                case RabbitExchangeProducerEndpoint exchangeEndpoint:
                    routingKey = route;
                    model.BasicPublish(
                        exchangeEndpoint.Name,
                        routingKey,
                        properties,
                        Bson.Serialize(value));
                    break;
                default:
                    throw new ArgumentException("Unhandled endpoint type.");
            }
        }

        public async Task ProduceAsync(object value, MessageHeaders headers) {
            await Task.Run(()=> Produce(value, headers));
        }

        public void Produce<T>(T value, MessageHeaders headers) where T : class, new() {
            Produce((object)value, headers);
        }

        public async Task ProduceAsync<T>(T value, MessageHeaders headers) where T : class, new() {
            await Task.Run(() => Produce(value, headers));
        }

        public void Dispose() {
            
        }
    }
}
