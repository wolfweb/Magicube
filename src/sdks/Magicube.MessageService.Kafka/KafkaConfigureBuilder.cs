using Confluent.Kafka;
using System;
using Magicube.Core;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Magicube.MessageService.Kafka {
    public class KafkaConfigureBuilder {
        private readonly IServiceCollection _servers;
        public KafkaConfigureBuilder(IServiceCollection servers) {
            _servers = servers;
        }

        public KafkaConfigureBuilder ConfigureProduce(Action<ProducerConfig> handler) {
            var config = new ProducerConfig();
            handler.Invoke(config);

            if (config.BootstrapServers.IsNullOrEmpty()) {
                Trace.WriteLine($"kafka produce need config bootstrap server address");
                _servers.Configure<KafkaOptions>(x => { });
            } else {
                _servers.Configure<KafkaOptions>(x => x.Produce = config);
            }
            return this;
        }

        public KafkaConfigureBuilder ConfigureConsume(Action<ConsumerConfig> handler) {
            var config = new ConsumerConfig();
            handler.Invoke(config);

            if (config.BootstrapServers.IsNullOrEmpty() || config.GroupId.IsNullOrEmpty()) {
                Trace.WriteLine($"kafka consume need config bootstrap server address and groupid");
                _servers.Configure<KafkaOptions>(x => { });
            } else {
                config.EnableAutoCommit = true;
                _servers.Configure<KafkaOptions>(x => x.Consumer = config);
            }

            return this;
        }
    }
}
