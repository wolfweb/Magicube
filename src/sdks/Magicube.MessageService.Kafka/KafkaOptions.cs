using Confluent.Kafka;

namespace Magicube.MessageService.Kafka {
    public class KafkaOptions {
        public ProducerConfig Produce  { get; set; }
        public ConsumerConfig Consumer { get; set; }
    }
}
