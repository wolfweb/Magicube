using Magicube.Cache.Redis;
using Magicube.Core;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace Magicube.MessageService.Redis {
    public class RedisMessageProducer : IProduceProvider {
        private readonly ILogger _logger;
        private readonly ISubscriber _subscriber;
        public RedisMessageProducer(
            ILogger<RedisMessageProducer> logger,
            IRedisResolve redisResolve
            ) {
            _logger     = logger;
            var conn    = redisResolve.GetConnectionMultiplexer();
            _subscriber = conn.GetSubscriber();
        }

        public void Produce(object value, MessageHeaders headers) {
            var message = ParseMessage(value, headers);

            if (headers.TryGetValue(MessageHeaders.MessageHeaderKey, out object v)) {
                _logger.LogDebug($"publish {v}");
                _subscriber.Publish(v.ToString(), Json.Stringify(message));
            }
        }

        public void Produce<T>(T value, MessageHeaders headers) where T : class, new() {
            Produce((object)value, headers);
        }

        public async Task ProduceAsync(object value, MessageHeaders headers) {
            var message = ParseMessage(value, headers);

            if (headers.TryGetValue(MessageHeaders.MessageHeaderKey, out object v)) {
                _logger.LogDebug($"publish {v}");
                await _subscriber.PublishAsync(v.ToString(), Json.Stringify(message));
            }
        }

        public async Task ProduceAsync<T>(T value, MessageHeaders headers) where T : class, new() {
            await ProduceAsync((object)value, headers);
        }

        private MessageBody ParseMessage(object value, MessageHeaders headers) => new MessageBody(value, headers);
    }
}
