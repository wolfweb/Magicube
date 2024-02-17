using Magicube.Core;

namespace Magicube.AI.Chat.Abstractions {
    public class AIChatException : MagicubeException {
        public AIChatException(string message) : base(10000, message) {
        }
    }
}