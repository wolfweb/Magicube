using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Magicube.Core.Convertion;
using Magicube.Core;
using System.Diagnostics;

namespace Magicube.MessageService.Kafka {
    public class KafkaProducer : IProduceProvider, IDisposable {
        private readonly KafkaOptions _kafkaOptions;
        private readonly ILogger<KafkaProducer> _logger;
        private IProducer<Null, byte[]> _producer;

        public KafkaProducer(IOptions<KafkaOptions> kafkaOptions, ILogger<KafkaProducer> logger) {
            _logger       = logger;
            _kafkaOptions = kafkaOptions.Value;

            Initialize();
        }

        public void Produce(object value, MessageHeaders headers) {
            if (_producer == null) return;
            var topic = headers.TryGet<string>(MessageHeaders.MessageHeaderKey);
            if (topic.IsNullOrEmpty()) {
                Trace.WriteLine("kafka produce need event topic");
                return;
            }
            _producer.Produce(topic, new Message<Null, byte[]>() {
                Value = Bson.Serialize(value)
            });
        }

        public async Task ProduceAsync(object value, MessageHeaders headers) {
            if (_producer == null) return;

            var topic = headers.TryGet<string>(MessageHeaders.MessageHeaderKey);
            if (topic.IsNullOrEmpty()) {
                Trace.WriteLine("");
                return;
            }
            await _producer.ProduceAsync(topic, new Message<Null, byte[]>() { 
                Value = Bson.Serialize(value)
            });
        }

        public void Produce<T>(T value, MessageHeaders headers) where T : class, new() {
            Produce((object)value, headers);
        }

        public async Task ProduceAsync<T>(T value, MessageHeaders headers) where T : class, new() {
            await ProduceAsync((object)value, headers);
        }

        public void Dispose() {
            _producer?.Dispose();
            GC.SuppressFinalize(this);
        }

        private void Initialize() {
            if (_kafkaOptions.Produce == null) {
                _logger.LogWarning($"cannot init kafka producer with no config!");
                return;
            }

            _producer = new ProducerBuilder<Null, byte[]>(_kafkaOptions.Produce).SetValueSerializer(Serializers.ByteArray).Build();
        }
    }
}
