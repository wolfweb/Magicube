using Magicube.Core.Modular;

namespace Magicube.OpenId {
    public class ModularInfo : ModularDescriptor {
        public const string Title = "开放认证";

        public override string Name    => "Magicube.OpenId";
        public override string Display => Title;
    }
}
