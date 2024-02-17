using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;

namespace Magicube.MessageService {
    public class MessageOptions {
        private readonly List<ConsumerConfigution> _configs;
        public MessageOptions() {
            _configs = new List<ConsumerConfigution>();
        }

        public IReadOnlyList<ConsumerConfigution> Consumers => _configs;

        public MessageOptions AddConsumer<T>(string key) {
            if (_configs.Any(x=>x.Key == key)) throw new MessageException($"the key : {key}'s consumer already exits");
            _configs.Add(new ConsumerConfigution(key, typeof(T)));
            return this;
        }
    }
    
    public sealed class ConsumerConfigution {
        public ConsumerConfigution(string key, Type type) { 
            Key          = key;
            ConsumerType = type;
        }
        public string Key          { get; }
        public Type   ConsumerType { get; }

        public bool IsSubOf<T>() {
            return ConsumerType.IsSubclassOf(typeof(T));
        }
    }

    public class DefaultMessageOptions : MessageOptions {
        public Channel<object> MessageServiceProvider { get; set; } = Channel.CreateUnbounded<object>();
    }
}
