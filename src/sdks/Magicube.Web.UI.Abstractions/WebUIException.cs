using Magicube.Core;

namespace Magicube.Web.UI {
    public class WebUIException : MagicubeException {
        private const int code = 26000;
        public WebUIException(string message) : base(code, message) {
        }
    }
}
