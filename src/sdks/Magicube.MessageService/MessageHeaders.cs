using Magicube.Core;
using System;

namespace Magicube.MessageService {
    public class MessageHeaders : TransferContext {
        public MessageHeaders() : base(StringComparer.OrdinalIgnoreCase) { 
        }
        public const string MessageHeaderKey = "Key";

        public static implicit operator MessageHeaders(string header) {
            return new MessageHeaders {
                [MessageHeaderKey] = header
            };
        }

        public bool HasTopicValue(string value) => ContainsKey(MessageHeaderKey) && this[MessageHeaderKey].Equals(value);
    }
}
