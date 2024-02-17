using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;

namespace Magicube.Web.UI {
    public interface IWidgetRenderProvider {
        Task<IHtmlContent> RenderAsync(IWidget widget);
    }
}
