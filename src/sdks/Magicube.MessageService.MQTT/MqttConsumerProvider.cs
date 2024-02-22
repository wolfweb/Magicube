using Magicube.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Magicube.MessageService.MQTT {
    public class MqttConsumerProvider : BaseConsumerProvider {
        private readonly IMqttClient _mqttClient;

        private readonly Channel<IMessageBody> _channel;
        private readonly ILogger<MqttConsumerProvider> _logger;

        private int _isStart = 0;

        public MqttConsumerProvider(
            ILogger<MqttConsumerProvider> logger, 
            IOptions<MessageOptions> options,
            IOptions<MqttOptions> mqttOptions,
            Application app) : base(options, app) {
            _logger      = logger;
            _channel     = Channel.CreateBounded<IMessageBody>(10);
            _mqttClient  = new MqttFactory().CreateMqttClient();
            _mqttClient.ApplicationMessageReceivedAsync += HandleApplicationMessageReceivedAsync;

            Task.WaitAll( _mqttClient.ConnectAsync(new MqttClientOptionsBuilder()
                .WithTcpServer(mqttOptions.Value.Server)
                .Build(), CancellationToken.None));
        }

        public async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs) {
            var receivedMessage = new MessageBody(eventArgs.ApplicationMessage.PayloadSegment, eventArgs.ApplicationMessage.Topic);
            await _channel.Writer.WriteAsync(receivedMessage).ConfigureAwait(false);
        }

        public override Task StartAsync(CancellationToken cancellationToken = default) {
            if(Interlocked.CompareExchange(ref _isStart, 1,0) == 0) {
                return Task.Factory.StartNew(async () => {
                    var msg = await _channel.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
                    using (var scope = Application.CreateScope()) {
                        var consumer = MatchConsumer(msg.Headers, scope);
                        if (consumer == null) {
                            Trace.WriteLine($"could not find consumer with header=>{msg.Headers}");
                            return;
                        }
                        msg.Headers.TryGetValue(MessageHeaders.MessageHeaderKey, out object topic);
                        Trace.WriteLine($"consume event message {topic} => Value: {msg.Value}");
                        await consumer.ConsumeAsync(msg, new MessageContext { 
                            Scope = scope
                        });
                    }

                }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            return Task.CompletedTask;
        }
    }
}
