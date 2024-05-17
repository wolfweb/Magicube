namespace Magicube.MessageService.RabbitMQ {
    public static class RabbitProducerExtension {
        public static IProduceProvider RabbitProduce(this IProduceProvider produce, object value, RabbitMessageHeaders header) {
            produce.Produce(value, header);
            return produce;
        }
    }
}
