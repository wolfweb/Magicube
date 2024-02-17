using Ganss.Xss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicube.Web.Html {
    public class HtmlSanitizerOptions {
        public List<Action<HtmlSanitizer>> Configure { get; } = new List<Action<HtmlSanitizer>>();
    }
}
