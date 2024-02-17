using Magicube.Storage.Abstractions;
using Magicube.Storage.Aliyun;
using Magicube.Storage.Upyun;
using Magicube.Storage.Tencent;
using Magicube.Web;
using Microsoft.Extensions.DependencyInjection;
using Magicube.Core.Modular;

namespace Magicube.Storage {
    public static class WebServiceBuilderExtension {
        public static WebServiceBuilder AddStorageWeb(this WebServiceBuilder builder) {
            builder.Services.Configure<ModularOptions>(options => {
                options.ModularShareAssembly.Add(typeof(WebServiceBuilderExtension).Assembly);
            });
            builder.Services.AddStorageCore();
            builder.Services.AddUpyunStorage();
            builder.Services.AddAliyunStorage();
            builder.Services.AddTencentStorage();

            return builder;
        }
    }
}