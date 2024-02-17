namespace Magicube.MessageService.RabbitMQ {
    public sealed class RabbitQueueConfig : RabbitEndpointConfig {
        public bool IsExclusive { get; set; }
    }
}
