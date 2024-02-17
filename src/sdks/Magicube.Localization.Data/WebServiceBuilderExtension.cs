using Magicube.Data.Abstractions;
using Magicube.Localization.Data;

namespace Magicube.Web {
    public static class WebServiceBuilderExtension {
        public static WebServiceBuilder AddStorageLocalizer(this WebServiceBuilder builder) {
            builder.AddJsonLocalization();
            builder.Services.AddEntity<LocalizerStore, LocalizerEntityMap>();
            return builder;
        }        
    }
}
