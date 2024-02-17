using Magicube.Core.Modular;
using Magicube.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using System;

namespace Magicube.Web {
    public static class WebServiceBuilderExtension {
        public static WebServiceBuilder AddJsonLocalization(this WebServiceBuilder builder, Action<RequestLocalizationOptions> handler = null) {
            builder.Services.Configure<ModularOptions>(x => x.ModularShareAssembly.Add(typeof(LocalizerService).Assembly));
            builder.Services.AddSingleton<LocalizerService>();
            builder.Services.AddSingleton<IHtmlLocalizerFactory, JsonHtmlLocalizerFactory>();
            builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            builder.Services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
            builder.Services.Configure<RequestLocalizationOptions>(options => {
                handler?.Invoke(options);
                options.FallBackToParentUICultures = true;
            });
            return builder;
        }
    }
}
