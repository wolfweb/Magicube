using Magicube.Data;
using Magicube.Data.Abstractions;
using Magicube.Web.UI.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magicube.Web.UI {
    public interface IWidgetService {
        Task<IWidget> GetWidget(string name);
    }

    public abstract class WidgetDataProvider : IWidgetService {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDynamicEntityRepository _dynamicEntityRepository;
        private readonly IRepository<WebWidget, int> _webWidgetRepository;

        public WidgetDataProvider(
            IHttpContextAccessor httpContextAccessor,
            IDynamicEntityRepository dynamicEntityRepository, 
            IRepository<WebWidget, int> webWidgetRepository) {
            _httpContextAccessor     = httpContextAccessor;
            _webWidgetRepository     = webWidgetRepository;
            _dynamicEntityRepository = dynamicEntityRepository;
        }

        public async Task<IWidget> GetWidget(string name) {
            var entity = await _webWidgetRepository.GetAsync(x => x.Name == name);
            return ParseWidget(entity);
        }

        public async Task<DynamicEntity> GetPage(string url, string widget, string entity) {
            var key = $"entity:{entity}";
            if (_httpContextAccessor.HttpContext.Items.ContainsKey(key)) {
                return _httpContextAccessor.HttpContext.Items[key] as DynamicEntity;
            }




            return null;
        }

        protected abstract IWidget ParseWidget(WebWidget entity);
    }

    public class PageWidgetDataBind {
        public string              Url    { get; set; }
        public string              Widget { get; set; }
        public string              Entity { get; set; }
        public IEnumerable<string> Fields { get; set; }
    }
}
