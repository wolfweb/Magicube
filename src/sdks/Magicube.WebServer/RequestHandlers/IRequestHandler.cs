using System.Text;
using System.Threading.Tasks;

namespace Magicube.WebServer.RequestHandlers {
    public interface IRequestHandler {
        string UrlPath { get; set; }

        EventHandler EventHandler { get; set; }

        Task<MiniWebContext> HandleRequestAsync(MiniWebContext context);

        Task<MiniWebContext> ProcessRequestAsync(MiniWebContext context);
    }
}
