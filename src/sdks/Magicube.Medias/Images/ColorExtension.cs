using System;
using System.Drawing;

namespace Magicube.Media.Images {
    public static class ColorExtension {
        public static String ToHexString(this Color c) {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}
