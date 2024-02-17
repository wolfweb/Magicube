using System;

namespace Magicube.Win {
    [Flags]
    public enum SplitChar {
        Equals = 0x1,
        Colon = 0x2,
        Any = Equals | Colon
    }

}
