using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Host {
    public class Program {
        public static void Main(string[] args) {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseKestrel(options => {
                options.ConfigureEndpointDefaults(endpointOptions => {
                    endpointOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                });
            })
            .UseStartup<Startup>();
    }
}
