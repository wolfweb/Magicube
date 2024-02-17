using System;

namespace Magicube.MessageService.Test {
    public class FooEventMessage {
        public string Name { get; set; } = Guid.NewGuid().ToString("n");
    }
}
