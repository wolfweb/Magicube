using Magicube.Core;
using Magicube.Data.Abstractions;

namespace Magicube.Web.Sites {
    public class BaseSiteModularConfigProvider<T> : IMagicubeConfigProvider<T> where T : class {
        private readonly ISiteManager _siteManager;

        public BaseSiteModularConfigProvider(ISiteManager siteManager) {
            _siteManager = siteManager;
        }

        public virtual T GetSetting() {
            return _siteManager.GetSite().As<T>() ?? default(T);
        }

        public virtual void SetSetting(T setting) {
            setting.NotNull();
            var siteSettings = _siteManager.GetSite();
            siteSettings.Put(setting);
            _siteManager.UpdateSite(siteSettings);
        }
    }
}
