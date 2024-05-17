using Magicube.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Magicube.MessageService {
    public interface IConsumerProvider {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync();
    }

    public abstract class BaseConsumerProvider : IConsumerProvider {
        protected const string ServiceScopeKey = "ServiceScope";
        protected readonly MessageOptions MessageOptions;
        protected readonly Application Application;
        protected BaseConsumerProvider(IOptions<MessageOptions> options, Application app) {
            Application    = app;
            MessageOptions = options.Value;
        }

        public abstract Task StartAsync(CancellationToken cancellationToken = default);
        public virtual Task StopAsync() => Task.CompletedTask;

        protected virtual IConsumer MatchConsumer(MessageHeaders headers, IServiceScope scope) {
            var key = headers[MessageHeaders.MessageHeaderKey].ToString();
            if (key.IsNullOrEmpty()) return default;

            var consumer = MessageOptions.Consumers.FirstOrDefault(x => x.Key == key);
            
            if (consumer == null) return default;
            return scope.ServiceProvider.Resolve<IConsumer>(consumer.ConsumerType);
        }
    }


    public class DefaultConsumerProvider : BaseConsumerProvider {
        private readonly ChannelReader<object> _reader;

        public DefaultConsumerProvider(
            IOptions<MessageOptions> options,
            IOptions<DefaultMessageOptions> defaultMessageoptions,
            Application app) : base(options, app) { 
            _reader = defaultMessageoptions.Value.MessageServiceProvider.Reader;
        }

        public override async Task StartAsync(CancellationToken cancellationToken = default) {
            while (await _reader.WaitToReadAsync(cancellationToken)) {
                var data = await _reader.ReadAsync(cancellationToken) as MessageBody;
                using (var scope = Application.CreateScope()) {
                    try {
                        var consumer = MatchConsumer(data.Headers, scope);
                        var _consumerStore = scope.ServiceProvider.GetService<IMessageStore>();
                        if (consumer != null) {
                            await _consumerStore.Deal(data);
                            await consumer.ConsumeAsync(data, new MessageContext {
                                Scope = scope
                            });
                        }
                    }catch(Exception e) {
                        Trace.WriteLine(e.ToString());
                    }
                }
            }
        }
    }
}
