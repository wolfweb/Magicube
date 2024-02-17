using Magicube.Core.Environment.Eventbus;

namespace Magicube.Eventbus {
    public class EntityActivity : IEventMessage {
        public EntityActivity(string name, object sender) {
            Name   = name;
            Sender = sender;
        }
        public string Name   { get; }

        public object Sender { get; }
    }
}
