using Magicube.Core.Runtime;
using Magicube.Core.Runtime.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace Magicube.Core.Test {
    public class RuntimeMetadataTest {
        private readonly IServiceProvider ServiceProvider;
        public RuntimeMetadataTest() {
            var services = new ServiceCollection()
                .AddCore()
                .Configure<StaticRuntimeMetadataOptions>(x=> {
                    
                })
                .AddTransient<IFooService, FooService>()
                .AddSingleton<RuntimeMetadataProvider>();

            ServiceProvider = services.AddSingleton(services).BuildServiceProvider();
        }

        [Fact]
        public void Func_RuntimeMetadata_Invokde_Test() {
            string name = "wolfweb";
            var runtimeMetadataProvider = ServiceProvider.GetRequiredService<RuntimeMetadataProvider>();
            var runtimeMethodMetadata = runtimeMetadataProvider.RuntimeMethods.FirstOrDefault(x => x.DeclaredType == typeof(IFooService));
            Assert.NotNull(runtimeMethodMetadata);
            runtimeMetadataProvider.Invoke(runtimeMethodMetadata, name);
            
            runtimeMethodMetadata = runtimeMetadataProvider.RuntimeMethods.FirstOrDefault(x => x.Title == "FooService:Concat");
            Assert.NotNull(runtimeMethodMetadata);
            var result = runtimeMetadataProvider.Invoke(runtimeMethodMetadata, "hello","world");
            Assert.NotNull(result);
            Assert.Equal("helloworld", result);
        }
    }

    public interface IFooService : IRuntimeMetadata {
        [RuntimeMethod(Title = "test", Descript = "test")]
        void Method(string v);

        [RuntimeMethod]
        string Concat(string a, string b);
    }
    public class FooService: IFooService {
        public void Method(string v) {
            Trace.WriteLine(v);
        }

        public string Concat(string a, string b) {
            return string.Concat(a, b);
        }
    }
}
