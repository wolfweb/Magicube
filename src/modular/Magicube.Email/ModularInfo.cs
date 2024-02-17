using Magicube.Core.Modular;

namespace Magicube.Email {
    public class ModularInfo : ModularDescriptor {
        public const string Title = "邮件";

        public override string  Name    => "Magicube.Email";
        public override string  Display => Title;
    }
}
