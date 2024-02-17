using Magicube.Core;

namespace Magicube.Cache.Abstractions {
    public class CacheException : MagicubeException {
        public CacheException(string message) : base(13000, message) {
        }
    }
}
