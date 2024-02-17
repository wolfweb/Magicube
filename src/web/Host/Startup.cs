using Magicube.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Host {
    public class Startup {
        public void ConfigureServices(IServiceCollection services) {
            services.AddMagicubeWeb(option=> {
                option.ModularFolder = @"E:\Proj\Magicube\src\modular";
                option.ThemeFolder   = @"E:\Proj\Magicube\src\themes";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseMagicubeWeb();
        }
    }
}
