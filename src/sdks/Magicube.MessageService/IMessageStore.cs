using System.Threading.Tasks;

namespace Magicube.MessageService {
    public interface IMessageStore {
        Task Storage<T>(T message) where T : IMessageBody;
        Task Deal<T>(T message) where T : IMessageBody;
    }

    public class NullProducerStore : IMessageStore {
        public Task Deal<T>(T message) where T : IMessageBody {
            return Task.CompletedTask;
        }

        public Task Storage<T>(T message) where T : IMessageBody {
            return Task.CompletedTask;
        }
    }
}
