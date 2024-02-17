using Magicube.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.MessageService {
    public class MessageContext {
        public MessageContext() {
            Properties = new TransferContext();
        }
        public IServiceScope    Scope      { get; set; }
        public TransferContext  Properties { get; }
    }
}
