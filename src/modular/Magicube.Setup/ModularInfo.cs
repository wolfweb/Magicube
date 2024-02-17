using Magicube.Core.Modular;

namespace Magicube.Setup {
    public class ModularInfo : ModularDescriptor {
        public const string ModularName = "Magicube.Setup";
        public override string Name    => ModularName;
        public override string Display => "设置";
    }
}
