using Magicube.Core;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Magicube.Net.Sms {
    public class SmsMessage {
        public string                      Text        { get; }
        public string                      PhoneNumber { get; }
        public IDictionary<string, object> Properties  { get; }

        public SmsMessage([NotNull] string phoneNumber, [NotNull] string text) {
            Text        = text.NotNullOrEmpty();
            PhoneNumber = phoneNumber.NotNullOrEmpty();

            Properties = new Dictionary<string, object>();
        }
    }
}
