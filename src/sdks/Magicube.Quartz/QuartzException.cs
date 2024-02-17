using Magicube.Core;
using System;

namespace Magicube.Quartz {
    public class QuartzException : MagicubeException {
        public QuartzException(string message) : base(18000, message) { }
    }
}
