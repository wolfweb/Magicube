using Magicube.Data;
using Magicube.Data.Abstractions;
using Magicube.Web.UI.Entities;
using Magicube.Web.UI.Liquid.Models;
using Microsoft.AspNetCore.Http;

namespace Magicube.Web.UI.Liquid.Services {
    public class LiquidWidgetDataProvider : WidgetDataProvider {
        public LiquidWidgetDataProvider(
            IHttpContextAccessor httpContextAccessor, 
            IDynamicEntityRepository dynamicEntityRepository, 
            IRepository<WebWidget, int> webWidgetRepository
            ) : base(httpContextAccessor, dynamicEntityRepository, webWidgetRepository) {
        }

        protected override IWidget ParseWidget(WebWidget entity) {
            return new LiquidWidgetModel {
                Name    = entity.Name,
                Content = entity.Content,
            };
        }
    }
}
