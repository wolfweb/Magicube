using Magicube.Core;

namespace Magicube.Media {
    public class MediaException : MagicubeException{
        public MediaException(string msg) : base(50000,msg) { }
    }
}
