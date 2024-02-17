using Magicube.Core;

namespace Magicube.MessageService {
    public class MessageException : MagicubeException {
        public MessageException(string message) : base(14000, message) {
        }
    }
}
