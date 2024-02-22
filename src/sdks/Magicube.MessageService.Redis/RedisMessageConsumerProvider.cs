using Magicube.Cache.Redis;
using Magicube.Core;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.MessageService.Redis {
    public class RedisMessageConsumerProvider : BaseConsumerProvider {
        private readonly ISubscriber _subscriber;

        public RedisMessageConsumerProvider(
            IOptions<MessageOptions> options,
            IRedisResolve redisResolve,
            Application app
            ) : base(options, app) {
            var conn = redisResolve.GetConnectionMultiplexer();
            _subscriber = conn.GetSubscriber();
        }

        public override async Task StartAsync(CancellationToken cancellationToken = default) {
            var topics = MessageOptions.Consumers.Select(x => x.Key);
            foreach (var topic in topics) {
                using (var scope = Application.CreateScope()) {
                    var consumer = MatchConsumer(topic, scope);
                    if (consumer == null) {
                        Trace.WriteLine($"not consumer for kafka with topic=>{topic}");
                        return;
                    }

                    await _subscriber.SubscribeAsync(topic, async (channel, value) => {
                        Trace.WriteLine($"consume event message {topic}, Value: {value}");
                        await consumer.ConsumeAsync(new MessageBody(value, topic), new MessageContext {
                            Scope = scope
                        });
                    });
                }
            }
        }
    }
}
