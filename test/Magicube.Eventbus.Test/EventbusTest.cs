using Magicube.Core;
using Magicube.Eventbus;
using Magicube.TestBase;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Magicube.Eventbus.Test {
    public class EventbusTest {
        [Fact]
        public void Func_Eventbus_Test() {
            var provider = new ServiceCollection()
                .AddCore()
                .AddEventCore()
                .AddEvent<Foo, FooCreated>()
                .AddEvent<Foo, FooCreating>()
                .BuildServiceProvider();

            var foo = new Foo { Name = "wolfweb" };

            var eventbusProvider = provider.GetRequiredService<IEventProvider>();

            var ctx = new EventContext<Foo>(foo);
            
            eventbusProvider.OnCreatingAsync(ctx);
            Assert.True(foo.State == -1);
            eventbusProvider.OnCreatedAsync(ctx);

            Assert.True(foo.State == 1);
        }
    }
    public class FooCreating : OnCreating<Foo> {
        public override Task OnHandlingAsync(EventContext<Foo> ctx) {
            ctx.Entity.State = -1;
            return Task.CompletedTask;
        }
    }

    public class FooCreated : OnCreated<Foo> {
        public override Task OnHandlingAsync(EventContext<Foo> ctx) {
            ctx.Entity.State = 1;
            return Task.CompletedTask;
        }
    }
}
