using Magicube.Core.Convertion;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using System.Threading.Tasks;

namespace Magicube.MessageService.MQTT {
    public class MqttProducer : IProduceProvider {
        private readonly IMqttClient _mqttClient;
        private readonly MqttOptions _mqttOptions;
        private readonly ILogger<MqttProducer> _logger;

        public MqttProducer(ILogger<MqttProducer> logger,IOptions<MqttOptions>options) {
            _logger      = logger;
            _mqttOptions = options.Value;

            _mqttClient = new MqttFactory().CreateMqttClient();
            Task.WaitAll(
            _mqttClient.ConnectAsync(new MqttClientOptionsBuilder()
                .WithTcpServer(_mqttOptions.Server)
                .Build()
                ));
        }

        public void Produce(object value, MessageHeaders headers) {
            var message = new MqttApplicationMessage { 
                Topic                 = headers.TryGet<string>(MessageHeaders.MessageHeaderKey),
                Retain                = _mqttOptions.Retain,
                Payload               = Bson.Serialize(value),
            };

            if (_mqttOptions.QualityOfServiceLevel.HasValue) message.QualityOfServiceLevel = _mqttOptions.QualityOfServiceLevel.Value;
            if (_mqttOptions.MessageExpiryInterval.HasValue) message.MessageExpiryInterval = _mqttOptions.MessageExpiryInterval.Value;

            _mqttClient.PublishAsync(message);
        }

        public async Task ProduceAsync(object value, MessageHeaders headers) {
            var message = new MqttApplicationMessage {
                Topic = headers.TryGet<string>(MessageHeaders.MessageHeaderKey),
                Payload = Bson.Serialize(value)
            };
            await _mqttClient.PublishAsync(message);
        }

        public void Produce<T>(T value, MessageHeaders headers) where T : class, new() {
            Produce((object)value, headers);
        }

        public async Task ProduceAsync<T>(T value, MessageHeaders headers) where T : class, new() {
            await ProduceAsync(value, headers);
        }

        public void Dispose() {
            _mqttClient?.Dispose();
        }

        
    }
}
