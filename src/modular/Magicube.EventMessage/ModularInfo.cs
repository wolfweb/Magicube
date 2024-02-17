using Magicube.Core.Modular;

namespace Magicube.MessageService {
    public class ModularInfo : ModularDescriptor {
        public const string Title = "消息服务";

        public override string Name    => "Magicube.MessageService";
        public override string Display => Title;
    }
}
