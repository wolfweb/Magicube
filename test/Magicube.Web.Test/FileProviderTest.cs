using Magicube.Core.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.IO;
using Xunit;
using Magicube.Core;

namespace Magicube.Web.Test {
    public class FileProviderTest {
        private readonly IServiceProvider ServiceProvider;
        public FileProviderTest() {
            var env = Mock.Of<IWebHostEnvironment>();

            env.WebRootPath = Path.GetTempPath();
            env.ContentRootPath = Path.GetTempPath();

            var services = new ServiceCollection();
            services
                .AddSingleton(env)
                .AddCore();

            ServiceProvider = services.BuildServiceProvider();
        }

        [Theory]
        [InlineData(@"c:\Users")]
        public void Func_FileProvider_Test(string dir) {
            var fileProvider = ServiceProvider.GetRequiredService<IWebFileProvider>();
            Assert.NotNull(fileProvider);

            var boolExpected = fileProvider.IsDirectory(dir);
            Assert.True(boolExpected);
        }
    }
}
