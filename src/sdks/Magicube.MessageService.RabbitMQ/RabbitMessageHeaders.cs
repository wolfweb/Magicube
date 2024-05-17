namespace Magicube.MessageService.RabbitMQ {
    public class RabbitMessageHeaders : MessageHeaders {
        public const string ExchangeKey = "EndpointName";
        public RabbitMessageHeaders(string exchange, string route = "") {
            Add(MessageHeaderKey, route);
            Add(ExchangeKey, exchange);
        }
    }
}
