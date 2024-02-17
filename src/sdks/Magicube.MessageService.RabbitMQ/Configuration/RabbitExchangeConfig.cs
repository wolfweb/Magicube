using Magicube.Core;
using ExchangeTypes = RabbitMQ.Client.ExchangeType;

namespace Magicube.MessageService.RabbitMQ {
    public class RabbitExchangeConfig : RabbitEndpointConfig {
        public string ExchangeType { get; set; }

        public override void Validate() {
            base.Validate();

            ExchangeType.NotNullOrEmpty();

            if (!ExchangeTypes.All().Contains(ExchangeType)) {
                throw new MessageException($"ExchangeType value is invalid. Allowed types are: ${string.Join(", ", ExchangeTypes.All())}.");
            }
        }
    }
}
