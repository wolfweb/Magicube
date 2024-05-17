using Magicube.Core;
using Magicube.TestBase;
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

        public string Key => Channel;

        public Task ConsumeAsync(IMessageBody message, MessageContext context) {
            _testOutputHelper.WriteLine($"receive-message foo=>{Json.Stringify(message.Value)}");
            _atomicCounter.Next();
            return Task.CompletedTask;
        }
    }
}
