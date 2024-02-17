using Microsoft.Extensions.Options;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Magicube.MessageService {
    public interface IProduceProvider {
        void Produce(object value, MessageHeaders headers);
        void Produce<T>(T value, MessageHeaders headers) where T : class, new();
        Task ProduceAsync(object value, MessageHeaders headers);
        Task ProduceAsync<T>(T value, MessageHeaders headers) where T : class, new();
    }

    public class DefaultProduceProvider : IProduceProvider {
        private readonly ChannelWriter<object> _writer;
        private readonly IMessageStore _producerStore;

        public DefaultProduceProvider(
            IOptions<DefaultMessageOptions> options,
            IMessageStore producerStore) {
            _writer = options.Value.MessageServiceProvider.Writer;
            _producerStore = producerStore;
        }

        public void Produce(object value, MessageHeaders headers) {
            var message = ParseMessage(value, headers);
            _producerStore.Storage(message);

            _writer.TryWrite(message);
        }

        public void Produce<T>(T value, MessageHeaders headers) where T : class, new() {
            Produce((object)value, headers);
        }

        public async Task ProduceAsync(object value, MessageHeaders headers) {
            var message = ParseMessage(value, headers);
            await _producerStore.Storage(message);
            await _writer.WriteAsync(message);
        }

        public async Task ProduceAsync<T>(T value, MessageHeaders headers) where T : class, new() {
            await ProduceAsync((object)value, headers);
        }

        private MessageBody ParseMessage(object value, MessageHeaders headers) => new MessageBody(value, headers);
    }
}
