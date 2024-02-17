using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Magicube.Core.Runtime;
using System.Linq;
using Magicube.TestBase;

namespace Magicube.Core.Test {
	public class ServiceCollectionTest {
		[Fact]
		public void Func_DI_Named_Test() {
			IServiceCollection services = new ServiceCollection();
			services
				.AddTransient<IFoo, FooA>(FooA.Identity)
				.AddTransient<IFoo, FooC>(FooC.Identity)
				.AddTransient<IFoo, FooB>(FooB.Identity);

			var provider = services.BuildServiceProvider();
			var foo = provider.GetService<IFoo>("B");
			Assert.NotNull(foo);

			var foos = provider.GetServices<IFoo>("A");
			Assert.True(foos.Any());
			Assert.True(foos.Count() == 2);
			Assert.Contains(foos, x => x.GetType() == typeof(FooA));
		}

		[Fact]
		public void Func_RuntimeMetadata_Test() {
			var serviceProvider = new ServiceCollection()
				.AddCore(builder=> {
					
				})
				.BuildServiceProvider();

			var runtimeMetadataProvider = serviceProvider.GetRequiredService<RuntimeMetadataProvider>();
			Assert.True(runtimeMetadataProvider.RuntimeMethods.Count() > 0);
		}
    }


	public class FooA : IFoo {
		public const string Identity = "A";
		public string Keyed => Identity;

        public string Name { get; set; }
    }

	public class FooB : IFoo  {
		public const string Identity = "B";
		public string Keyed => Identity;
		public string Name { get; set; }
	}
	
	public class FooC : IFoo {
		public const string Identity = "A";
		public string Keyed => Identity;
		public string Name { get; set; }
	}
}
