using Ganss.Xss;
using Microsoft.Extensions.Options;

namespace Magicube.Web.Html {
    public interface IHtmlSanitizerService {
        string Sanitize(string html);
    }

    public class HtmlSanitizerService : IHtmlSanitizerService {
        private readonly HtmlSanitizer _sanitizer = new HtmlSanitizer();

        public HtmlSanitizerService(IOptions<HtmlSanitizerOptions> options) {
            foreach (var action in options.Value.Configure) {
                action?.Invoke(_sanitizer);
            }
        }

        public string Sanitize(string html) {
            return _sanitizer.Sanitize(html);
        }
    }
}
