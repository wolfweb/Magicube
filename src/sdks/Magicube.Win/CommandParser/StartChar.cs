using System;

namespace Magicube.Win {
    [Flags]
    public enum StartChar {
        Slash = 0x1,
        Dash = 0x2,
        Any = Slash | Dash,
    }

}
