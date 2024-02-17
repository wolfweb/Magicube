using Magicube.Core;
using Magicube.MessageService.RabbitMQ.EndPoints;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;

namespace Magicube.MessageService.RabbitMQ {
    interface IRabbitConnectionFactory : IDisposable {
        IModel GetChannel(RabbitProducerEndpoint endpoint, string actualEndpointName);
        (IModel Channel, string QueueName) GetChannel(RabbitConsumerEndpoint endpoint);
    }

    class RabbitConnectionFactory : IRabbitConnectionFactory {
        private readonly ConcurrentDictionary<RabbitConnectionConfig, IConnection> _connections = new();

        public IModel GetChannel(RabbitProducerEndpoint endpoint, string actualEndpointName) {
            endpoint.NotNull();

            var channel = GetConnection(endpoint.Connection).CreateModel();

            if (endpoint.ConfirmationTimeout.HasValue)
                channel.ConfirmSelect();

            switch (endpoint) {
                case RabbitQueueProducerEndpoint queueEndpoint:
                    channel.QueueDeclare(
                        actualEndpointName,
                        queueEndpoint.Queue.IsDurable,
                        queueEndpoint.Queue.IsExclusive,
                        queueEndpoint.Queue.IsAutoDeleteEnabled,
                        queueEndpoint.Queue.Arguments);
                    break;
                case RabbitExchangeProducerEndpoint exchangeEndpoint:
                    channel.ExchangeDeclare(
                        actualEndpointName,
                        exchangeEndpoint.Exchange.ExchangeType,
                        exchangeEndpoint.Exchange.IsDurable,
                        exchangeEndpoint.Exchange.IsAutoDeleteEnabled,
                        exchangeEndpoint.Exchange.Arguments);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(endpoint), "Unexpected endpoint type.");
            }

            return channel;
        }

        public (IModel Channel, string QueueName) GetChannel(RabbitConsumerEndpoint endpoint) {
            endpoint.NotNull();

            var channel = GetConnection(endpoint.Connection).CreateModel();
            string queueName;

            switch (endpoint) {
                case RabbitQueueConsumerEndpoint queueEndpoint:
                    queueName = channel.QueueDeclare(
                            queueEndpoint.Name,
                            queueEndpoint.Queue.IsDurable,
                            queueEndpoint.Queue.IsExclusive,
                            queueEndpoint.Queue.IsAutoDeleteEnabled,
                            queueEndpoint.Queue.Arguments)
                        .QueueName;
                    break;
                case RabbitExchangeConsumerEndpoint exchangeEndpoint:
                    queueName = channel.QueueDeclare(
                            exchangeEndpoint.QueueName,
                            exchangeEndpoint.Queue.IsDurable,
                            exchangeEndpoint.Queue.IsExclusive,
                            exchangeEndpoint.Queue.IsAutoDeleteEnabled,
                            exchangeEndpoint.Queue.Arguments)
                        .QueueName;

                    channel.ExchangeDeclare(
                        exchangeEndpoint.Name,
                        exchangeEndpoint.Exchange.ExchangeType,
                        exchangeEndpoint.Exchange.IsDurable,
                        exchangeEndpoint.Exchange.IsAutoDeleteEnabled,
                        exchangeEndpoint.Exchange.Arguments);

                    channel.QueueBind(
                        queueName ?? string.Empty,
                        exchangeEndpoint.Name,
                        exchangeEndpoint.RoutingKey ?? string.Empty);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(endpoint), "Unexpected endpoint type.");
            }

            channel.BasicQos(endpoint.PrefetchSize, endpoint.PrefetchCount, false);

            return (channel, queueName!);
        }

        public void Dispose() {
            _connections?.Values.ForEach(connection => connection.Dispose());
        }

        private static IConnection CreateConnection(RabbitConnectionConfig connectionConfig) {
            var factory = new ConnectionFactory {
                DispatchConsumersAsync = true
            };

            factory.ApplyConfiguration(connectionConfig);

            return factory.CreateConnection();
        }

        private IConnection GetConnection(RabbitConnectionConfig connectionConfig) {
            connectionConfig.NotNull();

            if (_connections == null)
                throw new ObjectDisposedException(null);

            lock (_connections) {
                if (_connections.TryGetValue(connectionConfig, out IConnection connection))
                    return connection;

                connection = CreateConnection(connectionConfig);
                _connections.TryAdd(connectionConfig, connection);

                return connection;
            }
        }
    }

    internal static class RabbitConnectionFactoryExtensions {
        public static void ApplyConfiguration(this ConnectionFactory connectionFactory, RabbitConnectionConfig config) {
            connectionFactory.NotNull();
            config.NotNull();

            connectionFactory
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.AmqpUriSslProtocols,
                    (factory, value) => factory.AmqpUriSslProtocols = value)
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.AutomaticRecoveryEnabled,
                    (factory, value) => factory.AutomaticRecoveryEnabled = value)
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.HostName,
                    (factory, value) => factory.HostName = value)
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.NetworkRecoveryInterval,
                    (factory, value) => factory.NetworkRecoveryInterval = value)
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.HandshakeContinuationTimeout,
                    (factory, value) => factory.HandshakeContinuationTimeout = value)
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.ContinuationTimeout,
                    (factory, value) => factory.ContinuationTimeout = value)
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.Port,
                    (factory, value) => factory.Port = value)
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.RequestedConnectionTimeout,
                    (factory, value) => factory.RequestedConnectionTimeout = value)
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.SocketReadTimeout,
                    (factory, value) => factory.SocketReadTimeout = value)
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.SocketWriteTimeout,
                    (factory, value) => factory.SocketWriteTimeout = value)
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.TopologyRecoveryEnabled,
                    (factory, value) => factory.TopologyRecoveryEnabled = value)
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.ClientProperties,
                    (factory, value) => factory.ClientProperties = value)
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.Password,
                    (factory, value) => factory.Password = value)
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.RequestedChannelMax,
                    (factory, value) => factory.RequestedChannelMax = value)
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.RequestedFrameMax,
                    (factory, value) => factory.RequestedFrameMax = value)
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.RequestedHeartbeat,
                    (factory, value) => factory.RequestedHeartbeat = value)
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.UseBackgroundThreadsForIO,
                    (factory, value) => factory.UseBackgroundThreadsForIO = value)
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.UserName,
                    (factory, value) => factory.UserName = value)
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.VirtualHost,
                    (factory, value) => factory.VirtualHost = value)
                .ApplyConfigIfNotNull(
                    config,
                    sourceConfig => sourceConfig.ClientProvidedName,
                    (factory, value) => factory.ClientProvidedName = value)
                .Ssl.ApplyConfiguration(config.Ssl);
        }

        private static void ApplyConfiguration(this SslOption destination, RabbitSslOption source) {
            destination.NotNull();
            source.NotNull();

            destination
                .ApplyConfigIfNotNull(
                    source,
                    sourceConfig => sourceConfig.AcceptablePolicyErrors,
                    (factory, value) => factory.AcceptablePolicyErrors = value)
                .ApplyConfigIfNotNull(
                    source,
                    sourceConfig => sourceConfig.CertPassphrase,
                    (factory, value) => factory.CertPassphrase = value)
                .ApplyConfigIfNotNull(
                    source,
                    sourceConfig => sourceConfig.CertPath,
                    (factory, value) => factory.CertPath = value)
                .ApplyConfigIfNotNull(
                    source,
                    sourceConfig => sourceConfig.CheckCertificateRevocation,
                    (factory, value) => factory.CheckCertificateRevocation = value)
                .ApplyConfigIfNotNull(
                    source,
                    sourceConfig => sourceConfig.Enabled,
                    (factory, value) => factory.Enabled = value)
                .ApplyConfigIfNotNull(
                    source,
                    sourceConfig => sourceConfig.ServerName,
                    (factory, value) => factory.ServerName = value)
                .ApplyConfigIfNotNull(
                    source,
                    sourceConfig => sourceConfig.Version,
                    (factory, value) => factory.Version = value);
        }

        private static TDestination ApplyConfigIfNotNull<TSource, TDestination, TValue>(
            this TDestination destination,
            TSource source,
            Func<TSource, TValue> sourceFunction,
            Action<TDestination, TValue> applyAction) {
            var value = sourceFunction(source);

            if (value != null)
                applyAction(destination, value);

            return destination;
        }

        private static TDestination ApplyConfigIfNotNull<TSource, TDestination, TValue>(
            this TDestination destination,
            TSource source,
            Func<TSource, TValue?> sourceFunction,
            Action<TDestination, TValue> applyAction)
            where TValue : struct {
            var value = sourceFunction(source);

            if (value.HasValue)
                applyAction(destination, value.Value);

            return destination;
        }
    }
}
