using Magicube.MessageService.RabbitMQ.EndPoints;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Magicube.MessageService.RabbitMQ {
    public class RabbitMessageBuilder {
        private readonly IServiceCollection _services;
        private IRabbitMessageServiceBuilder _rabbitMessageServiceBuilder;
        public RabbitMessageBuilder(IServiceCollection services){
            _services = services;
            _services.AddSingleton(this);
        }

        public RabbitProducerEndpoint ProducerEndpoint => _rabbitMessageServiceBuilder.ProducerEndpoint;

        public List<RabbitConsumerEndpoint> ConsumerEndpoints => _rabbitMessageServiceBuilder.ConsumerEndpoints;

        public IRabbitMessageExchangeServiceBuilder ConfigExchange(Action<IRabbitMessageExchangeServiceBuilder> config) {
            var builder = new RabbitMessageExchangeBuilder();
            config?.Invoke(builder);
            _rabbitMessageServiceBuilder = builder;
            return builder;
        }

        public IRabbitMessageServiceBuilder ConfigQueue(Action<IRabbitMessageQueueServiceBuilder> config) {
            var builder = new RabbitMessageQueueBuilder();
            config?.Invoke(builder);
            _rabbitMessageServiceBuilder = builder;
            return _rabbitMessageServiceBuilder;
        }

        abstract class RabbitMessageServiceBuilder<TProducer, TConsumer> : IRabbitMessageServiceBuilder
        where TProducer : RabbitProducerEndpoint
        where TConsumer : RabbitConsumerEndpoint {

            public IRabbitMessageServiceBuilder ConfigConsumer(string name, Action<RabbitConsumerEndpoint> config) {
                return InternalConfigConsumer(name, config);
            }

            public IRabbitMessageServiceBuilder ConfigProducer(string name, Action<RabbitProducerEndpoint> config) {
                return InternalConfigProducer(name, config);
            }

            public abstract RabbitProducerEndpoint ProducerEndpoint { get; }

            public abstract List<RabbitConsumerEndpoint> ConsumerEndpoints { get; }

            protected abstract IRabbitMessageServiceBuilder InternalConfigProducer(string name, Action<TProducer> config);

            protected abstract IRabbitMessageServiceBuilder InternalConfigConsumer(string name, Action<TConsumer> config);
        }

        sealed class RabbitMessageExchangeBuilder : RabbitMessageServiceBuilder<RabbitExchangeProducerEndpoint, RabbitExchangeConsumerEndpoint>, IRabbitMessageExchangeServiceBuilder {
            private readonly List< RabbitConsumerEndpoint> _rabbitConsumerEndpoints;

            private RabbitProducerEndpoint _rabbitProducerEndpoint;
            private RabbitConnectionConfig _rabbitConnectionConfig;

            public RabbitMessageExchangeBuilder() {
                _rabbitConsumerEndpoints = new List<RabbitConsumerEndpoint>();
            }

            public RabbitExchangeConfig Exchange { get; } = new();

            public override RabbitProducerEndpoint ProducerEndpoint {
                get {
                    if (_rabbitConnectionConfig != null) {
                        _rabbitProducerEndpoint.Connection = _rabbitConnectionConfig;
                    }
                    return _rabbitProducerEndpoint;
                }
            }
            public override List<RabbitConsumerEndpoint> ConsumerEndpoints {
                get {
                    if (_rabbitConnectionConfig != null) {
                        _rabbitConsumerEndpoints.ForEach(x => x.Connection = _rabbitConnectionConfig);
                    }
                    return _rabbitConsumerEndpoints;
                }
            }

            protected override IRabbitMessageServiceBuilder InternalConfigProducer(string name, Action<RabbitExchangeProducerEndpoint> config) {
                var endpoint = new RabbitExchangeProducerEndpoint(name);
                endpoint.Exchange = Exchange;
                config?.Invoke(endpoint);
                _rabbitProducerEndpoint = endpoint;
                return this;
            }

            protected override IRabbitMessageServiceBuilder InternalConfigConsumer(string name, Action<RabbitExchangeConsumerEndpoint> config) {
                var endpoint = new RabbitExchangeConsumerEndpoint(name);
                endpoint.Exchange = Exchange;
                config?.Invoke(endpoint);
                _rabbitConsumerEndpoints.Add(endpoint);
                return this;
            }

            public IRabbitMessageExchangeServiceBuilder ConfigConnection(Action<RabbitConnectionConfig> conn) {
                _rabbitConnectionConfig = new RabbitConnectionConfig();
                conn?.Invoke(_rabbitConnectionConfig);
                return this;
            }

            public IRabbitMessageExchangeServiceBuilder ConfigProducer(string name, Action<RabbitExchangeProducerEndpoint> config) {
                InternalConfigProducer(name, config);
                return this;
            }

            public IRabbitMessageExchangeServiceBuilder ConfigConsumer(string name, Action<RabbitExchangeConsumerEndpoint> config) {
                InternalConfigConsumer(name, config);
                return this;
            }
        }

        sealed class RabbitMessageQueueBuilder : RabbitMessageServiceBuilder<RabbitQueueProducerEndpoint, RabbitQueueConsumerEndpoint>, IRabbitMessageQueueServiceBuilder {
            private readonly List<RabbitConsumerEndpoint> _rabbitConsumerEndpoints;

            private RabbitProducerEndpoint _rabbitProducerEndpoint;
            private RabbitConnectionConfig _rabbitConnectionConfig;

            public RabbitMessageQueueBuilder() {
                _rabbitConsumerEndpoints = new();
            }

            public override RabbitProducerEndpoint ProducerEndpoint {
                get {
                    if (_rabbitConnectionConfig != null) {
                        _rabbitProducerEndpoint.Connection = _rabbitConnectionConfig;
                    }
                    return _rabbitProducerEndpoint;
                }
            }
            public override List<RabbitConsumerEndpoint> ConsumerEndpoints {
                get {
                    if (_rabbitConnectionConfig != null) {
                        _rabbitConsumerEndpoints.ForEach(x => x.Connection = _rabbitConnectionConfig);
                    }
                    return _rabbitConsumerEndpoints;
                }
            }

            protected override IRabbitMessageServiceBuilder InternalConfigProducer(string name, Action<RabbitQueueProducerEndpoint> config) {
                var endpoint = new RabbitQueueProducerEndpoint(name);
                config?.Invoke(endpoint);
                _rabbitProducerEndpoint = endpoint;
                return this;
            }

            protected override IRabbitMessageServiceBuilder InternalConfigConsumer(string name, Action<RabbitQueueConsumerEndpoint> config) {
                var endpoint = new RabbitQueueConsumerEndpoint(name);
                config?.Invoke(endpoint);
                _rabbitConsumerEndpoints.Add(endpoint);
                return this;
            }

            public IRabbitMessageQueueServiceBuilder ConfigConnection(Action<RabbitConnectionConfig> conn) {
                _rabbitConnectionConfig = new RabbitConnectionConfig();
                conn?.Invoke(_rabbitConnectionConfig);
                return this;
            }

            public IRabbitMessageQueueServiceBuilder ConfigProducer(string name, Action<RabbitQueueProducerEndpoint> config) {
                InternalConfigProducer(name, config);
                return this;
            }

            public IRabbitMessageQueueServiceBuilder ConfigConsumer(string name, Action<RabbitQueueConsumerEndpoint> config) {
                InternalConfigConsumer(name, config);
                return this;
            }
        }
    }

    public interface IRabbitMessageServiceBuilder {
        IRabbitMessageServiceBuilder ConfigProducer(string name, Action<RabbitProducerEndpoint> config);
        IRabbitMessageServiceBuilder ConfigConsumer(string name, Action<RabbitConsumerEndpoint> config);

        RabbitProducerEndpoint ProducerEndpoint        { get; }

        List<RabbitConsumerEndpoint> ConsumerEndpoints { get; }
    }

    public interface IRabbitMessageExchangeServiceBuilder {
        RabbitExchangeConfig Exchange { get; }
        IRabbitMessageExchangeServiceBuilder ConfigConnection(Action<RabbitConnectionConfig> conn);
        IRabbitMessageExchangeServiceBuilder ConfigProducer(string name, Action<RabbitExchangeProducerEndpoint> config);
        IRabbitMessageExchangeServiceBuilder ConfigConsumer(string name, Action<RabbitExchangeConsumerEndpoint> config);
    }
    
    public interface IRabbitMessageQueueServiceBuilder {
        IRabbitMessageQueueServiceBuilder ConfigConnection(Action<RabbitConnectionConfig> conn);
        IRabbitMessageQueueServiceBuilder ConfigProducer(string name, Action<RabbitQueueProducerEndpoint> config);
        IRabbitMessageQueueServiceBuilder ConfigConsumer(string name, Action<RabbitQueueConsumerEndpoint> config);
    }     
}
