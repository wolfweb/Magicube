using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Magicube.Core;

namespace Magicube.MessageService.Kafka {
    public class KafkaConsumerProvider : BaseConsumerProvider, IDisposable {
        private readonly ILogger _logger;
        private readonly KafkaOptions _kafkaOptions;
        private IConsumer<Ignore, string> _consumer;
        private int _isStart = 0;

        public KafkaConsumerProvider(
            Application app,
            IOptions<MessageOptions> options,
            IOptions<KafkaOptions> kafkaOptions,
            ILogger<KafkaConsumerProvider> logger
            ) : base(options, app) {
            _logger             = logger;
            _kafkaOptions       = kafkaOptions.Value;
            Initialize();
        }
        
        public void Dispose() {
            if (_consumer != null) {
                _consumer.Dispose();
                _consumer = null;
            }
            GC.SuppressFinalize(this);
        }

        public override Task StartAsync(CancellationToken cancellationToken = default) {
            if (Interlocked.CompareExchange(ref _isStart, 1, 0) == 0) {
                Task.Factory.StartNew(() => {
                    var message = _consumer.Consume(cancellationToken);
                    Trace.WriteLine($"receive {message.Topic} with offset {message.Offset} on partition {message.Partition}");
                    
                    MessageHeaders headers = message.Topic;
                    headers.Add("Offset", message.Offset);
                    headers.Add("Partition", message.Partition);

                    using (var scope = Application.CreateScope()) {
                        var consumer = MatchConsumer(headers, scope);
                        if (consumer == null) {
                            Trace.WriteLine($"not consumer for kafka with topic=>{message.Topic}");
                            return;
                        }
                        Trace.WriteLine($"consume event message {message.Topic} => Partition: {message.Partition}, Offset: {message.Offset}, Value: {message.Message.Value}");
                        consumer.ConsumeAsync(new MessageBody(message.Message.Value, headers), new MessageContext {
                            Scope = scope
                        });
                    }

                }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            return Task.CompletedTask;
        }

        public override Task StopAsync() {
            return Task.CompletedTask;
        }

        private void Initialize() {
            if (_kafkaOptions.Produce == null) {
                _logger.LogWarning($"cannot init kafka producer with no config!");
                return;
            }

            _consumer = new ConsumerBuilder<Ignore, string>(_kafkaOptions.Consumer)
                .SetErrorHandler((_, e) => Trace.WriteLine($"Error: {e}"))
                .SetPartitionsAssignedHandler((_, partitions) => Trace.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}], member id: {_consumer.MemberId}"))
                .SetPartitionsRevokedHandler((_, partitions) => Trace.WriteLine($"Revoked partitions: [{string.Join(", ", partitions)}]"))
                .SetOffsetsCommittedHandler((_, commit) => Trace.WriteLine(commit.Error.IsError ? $"Failed to commit offsets: {commit.Error}" : $"Successfully committed offsets: [{string.Join(", ", commit.Offsets)}]"))
                .SetValueDeserializer(Deserializers.Utf8).Build();

            _consumer.Subscribe(MessageOptions.Consumers.Select(x => x.Key));
        }
    }
}
