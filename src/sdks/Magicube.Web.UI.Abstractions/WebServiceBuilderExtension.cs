using Magicube.Core.Modular;
using Magicube.Data.Abstractions;
using Magicube.Web.UI;
using Magicube.Web.UI.Entities;
using Magicube.Web.UI.TagHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Web {
    public static class WebServiceBuilderExtension {
        public static WebServiceBuilder AddWebUICore(this WebServiceBuilder builder) {
            builder.Services.Configure<ModularOptions>(options => {
                options.ModularShareAssembly.Add(typeof(WebUIException).Assembly);
            });
            
            AddDynamicTagHelpers(builder.Services);
            AddWebUIEntities(builder.Services);

            return builder;
        }

        private static void AddWebUIEntities(IServiceCollection services) {
            services
                .AddEntity<WebWidget, WebWidgetEntityMapping>()
                .AddEntity<WebLayout, WebLayoutEntityMapping>()
                .AddEntity<WebPage, WebPageEntityMapping>();
        }

        private static void AddDynamicTagHelpers(IServiceCollection services) {
            services.AddTransient<FormGroupBuilder>();
            services.AddTransient<IElementBuilder, InputElement>();
            services.AddTransient<IElementBuilder, TextareaElement>();
            services.AddTransient<IElementBuilder, CheckboxElement>();
            services.AddTransient<IElementBuilder, RadioElement>();
            services.AddTransient<IElementBuilder, SelectElement>();
            services.AddTransient<IElementBuilder, HiddenElement>();
            services.AddTransient<IElementBuilder, RangeElement>();
            services.AddTransient<IElementBuilder, ArrayStringElement>();
        }
    }
}
