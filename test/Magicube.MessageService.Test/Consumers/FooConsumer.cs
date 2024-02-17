using Magicube.TestBase;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Magicube.MessageService.Test {
    sealed class FooConsumer : IConsumer {
        private readonly AtomicCounter _atomicCounter;
        private readonly ITestOutputHelper _testOutputHelper;

        public FooConsumer(ITestOutputHelper testOutputHelper, AtomicCounter atomicCounter) {
            _testOutputHelper = testOutputHelper;
            _atomicCounter = atomicCounter;
        }

        public const string Channel = "foo";

        public string Topic => Channel;

        public Task ConsumeAsync(IMessageBody message, MessageContext context) {
            _testOutputHelper.WriteLine($"redis-message foo=>{message.Value}");
            Trace.WriteLine($"receive redis message foo=> ${message.Value}");
            _atomicCounter.Next();
            return Task.CompletedTask;
        }
    }
}
