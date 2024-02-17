namespace Magicube.MessageService {
    public interface IMessageBody {
        object         Value   { get; }
        MessageHeaders Headers { get; }
    }

    public class MessageBody : IMessageBody {
        public MessageBody(object value, MessageHeaders headers) {
            Value   = value;
            Headers = headers;
        }
        public object         Value   { get; }

        public MessageHeaders Headers { get; }
    }
}
