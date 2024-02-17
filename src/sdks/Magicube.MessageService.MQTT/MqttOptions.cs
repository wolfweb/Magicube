using MQTTnet.Protocol;

namespace Magicube.MessageService.MQTT {
    public class MqttOptions {
        public bool                       Retain                { get; set; }
        public string                     Server                { get; set; }
        public uint?                      MessageExpiryInterval { get; set; }
        public MqttQualityOfServiceLevel? QualityOfServiceLevel { get; set; }
    }
}
