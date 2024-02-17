using System.Collections.Generic;

namespace Magicube.MessageService.RabbitMQ {
    public abstract class RabbitEndpointConfig {
        public bool                       IsDurable            { get; set; } = true;                      
                                         
        public bool                       IsAutoDeleteEnabled  { get; set; }

        public Dictionary<string, object> Arguments            { get; set; }

        public virtual void Validate() {
        }
    }
}
