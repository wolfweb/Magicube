using System.Threading.Tasks;

namespace Magicube.MessageService {
    public interface IConsumer{
        Task ConsumeAsync(IMessageBody message, MessageContext ctx);
    }
}
