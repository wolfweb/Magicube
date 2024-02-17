using Magicube.Core.Modular;
using System;

namespace DefaultTheme {
    public class Modular : ModularDescriptor {
        public override string  Name    => "DefaultTheme";
        public override string  Display => "主题";

        public override Version Version => new Version(0, 0, 1);
    }
}
