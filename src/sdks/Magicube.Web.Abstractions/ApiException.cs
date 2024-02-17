using Magicube.Core;

namespace Magicube.Web {
    public class ApiException : MagicubeException {
        public const int ApiCode = 20000;
        public ApiException(string message) : base(ApiCode, message) {
        }
    }
}
