using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magicube.Core.Environment.Eventbus {
    public class EventbusOption {
        public Dictionary<Type, List<IEventsubscriber>> Subscribers { get; set; }
    }
    public interface IEventbus {
        Task Publish(IEventMessage activity);
        Task Subscribe<T,TMsg>() where T : IEventsubscriber where TMsg : IEventMessage;
    }

    public class EventbusProvider : IEventbus {
        private readonly Dictionary<Type, List<IEventsubscriber>> _subscribers;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public EventbusProvider(ILogger<EventbusProvider> logger, IOptions<EventbusOption> options, IServiceProvider serviceProvider) {
            _logger           = logger;
            _serviceProvider  = serviceProvider;
            _subscribers      = options?.Value?.Subscribers ?? new Dictionary<Type, List<IEventsubscriber>>();
        }

        public async Task Publish(IEventMessage activity) {
            var type = activity.GetType();
            if (_subscribers.ContainsKey(type)) {
                await _subscribers[type].InvokeAsync(x => x.Invoke(activity), _logger);
            }
        }

        public Task Subscribe<T,TMsg>() where T : IEventsubscriber where TMsg : IEventMessage {
            var type = typeof(TMsg);
            if (_subscribers.ContainsKey(type)) {
                _subscribers[type].Add(ActivatorUtilities.CreateInstance<T>(_serviceProvider));
            } else {
                _subscribers.Add(type, new List<IEventsubscriber> {
                    ActivatorUtilities.CreateInstance<T>(_serviceProvider)
                });
            }
            return Task.CompletedTask;
        }
    }
}
