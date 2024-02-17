using Magicube.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Magicube.Net.Test {
    public class CurlTest {
        private readonly ServiceProvider serviceProvider;
        private readonly ITestOutputHelper _testOutputHelper;

        private const string url = "http://cn.bing.com";
        public CurlTest(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
            var services = new ServiceCollection();
            services
                .AddCore()
                .AddHttpServices();
            serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async Task Curl_Post_Test() {
            var curl = serviceProvider.GetRequiredService<Curl>();
            var result = await curl.Post(url, Curl.FORM, "").ReadAsString();
            _testOutputHelper.WriteLine(result);
            Assert.True(!result.IsNullOrEmpty());
        }

        [Fact]
        public async Task Curl_Get_Test() {
            var curl = serviceProvider.GetRequiredService<Curl>();
            var result = await curl.Get(url).ReadAsString();
            _testOutputHelper.WriteLine(result);
            Assert.True(!result.IsNullOrEmpty());
        }

        [Fact]
        public async Task Curl_Get_With_Test() {
            var curl = serviceProvider.GetRequiredService<Curl>();
            curl.Initialize(client => {
                client.DefaultRequestHeaders.Add("User-Agent", "Magicube");
            });
            var result = await curl.Get(url).ReadAsString();
            _testOutputHelper.WriteLine(result);
            Assert.True(!result.IsNullOrEmpty());
        }

        [Fact]
        public async Task Curl_Get_Work_Test() {
            var curl = serviceProvider.GetRequiredService<Curl>();
            var result = await curl.Get(url).ReadAsString();
            _testOutputHelper.WriteLine(result);
            Assert.True(!result.IsNullOrEmpty());
        }
    }
}
