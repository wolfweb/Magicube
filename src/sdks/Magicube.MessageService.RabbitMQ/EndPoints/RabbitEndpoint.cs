using System;

namespace Magicube.MessageService.RabbitMQ.EndPoints {
    public abstract class RabbitEndpoint : IEquatable<RabbitEndpoint> {
        public string                 Name       { get; }
        public RabbitConnectionConfig Connection { get; set; } = new();

        public RabbitEndpoint(string name) {
            Name = name;
        }

        public bool Equals(RabbitEndpoint other) {
            if (ReferenceEquals(this, other))
                return true;

            return Equals(Connection, other.Connection);
        }
    }

    public abstract class RabbitProducerEndpoint : RabbitEndpoint {
        public TimeSpan? ConfirmationTimeout { get; set; } = TimeSpan.FromSeconds(5);

        public RabbitProducerEndpoint(string name) : base(name) {
        }
    }

    public sealed class RabbitExchangeProducerEndpoint : RabbitProducerEndpoint, IEquatable<RabbitExchangeProducerEndpoint> {
        public RabbitExchangeConfig Exchange { get; set; } = new();

        public RabbitExchangeProducerEndpoint(string name) : base(name) {
        }

        public bool Equals(RabbitExchangeProducerEndpoint other) {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Equals(other) && Equals(Exchange, other.Exchange);
        }
    }

    public sealed class RabbitQueueProducerEndpoint : RabbitProducerEndpoint, IEquatable<RabbitQueueProducerEndpoint> {
        public RabbitQueueConfig Queue { get; set; } = new();

        public RabbitQueueProducerEndpoint(string name) : base(name) {
        }

        public bool Equals(RabbitQueueProducerEndpoint other) {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Equals(Queue, other.Queue);
        }
    }

    public abstract class RabbitConsumerEndpoint : RabbitEndpoint {
        public RabbitConsumerEndpoint(string name) : base(name) {
        }

        public RabbitQueueConfig Queue                { get; set; } = new();

        public int               AcknowledgeEach      { get; set; } = 1;
                                 
        public bool              AutoAck              { get; set; } = true;
                                 
        public uint              PrefetchSize         { get; set; }
                                 
        public ushort            PrefetchCount        { get; set; }
                                 
        public ushort            CunsumerMaxBatchSize { get; internal set; }
    }

    public sealed class RabbitExchangeConsumerEndpoint : RabbitConsumerEndpoint, IEquatable<RabbitExchangeConsumerEndpoint> {
        public RabbitExchangeConsumerEndpoint(string name) : base(name) {
        }

        public RabbitExchangeConfig Exchange   { get; set; } = new();

        public string               QueueName  { get; set; }
                                    
        public string               RoutingKey { get; set; }

        public bool Equals(RabbitExchangeConsumerEndpoint other) {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Equals(Exchange, other.Exchange) &&
                   string.Equals(QueueName, other.QueueName, StringComparison.Ordinal) &&
                   string.Equals(RoutingKey, other.RoutingKey, StringComparison.Ordinal);
        }
    }

    public sealed class RabbitQueueConsumerEndpoint : RabbitConsumerEndpoint, IEquatable<RabbitQueueConsumerEndpoint> {
        public RabbitQueueConsumerEndpoint(string name) : base(name) {
        }

        public bool Equals(RabbitQueueConsumerEndpoint other) {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Equals(Queue, other.Queue);
        }
    }
}
