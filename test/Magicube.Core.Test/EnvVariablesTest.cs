using Magicube.Core.Environment.Variable;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Magicube.Core.Test {
    public class EnvVariablesTest {
        private IServiceProvider ServiceProvider;
        public EnvVariablesTest() {
            ServiceProvider = new ServiceCollection()
                .AddCore()
                .BuildServiceProvider();
        }

        [Theory]
        [InlineData("/user/{PATH_NAME}/{FILE_NAME}", "abaa/123/ab.jpg", "/user/123/ab.jpg")]
        [InlineData("/user/{DATE}/{file_name}", "abaa/123/ab.jpg", "/user/20230819/ab.jpg")]
        public void Env_Variables_Test(string template, string value, string expected) {
            var factory = ServiceProvider.GetRequiredService<VariableFactroy>();
            var actual = factory.Parse(template, value);
            Assert.Equal(expected, actual);
        }
    }
}
