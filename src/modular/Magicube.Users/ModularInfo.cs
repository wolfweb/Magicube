using Magicube.Core.Modular;

namespace Magicube.Users {
    public class ModularInfo : ModularDescriptor {
        public const string    Title = "用户";
        public const string    ModularName = "Magicube.Users";

        public override string Name        => ModularName;
        public override string Display     => Title;
    }
}
