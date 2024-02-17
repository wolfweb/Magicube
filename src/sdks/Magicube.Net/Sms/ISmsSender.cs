using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Magicube.Net.Sms {
    public interface ISmsSender {
        Task SendAsync(SmsMessage smsMessage);
    }

    public class NullSmsSender : ISmsSender {
        public ILogger Logger { get; set; }

        public NullSmsSender(ILogger<NullSmsSender> logger) {
            Logger = logger;
        }

        public Task SendAsync(SmsMessage smsMessage) {
            Logger.LogWarning($"SMS Sending was not implemented! Using {nameof(NullSmsSender)}:");

            Logger.LogWarning("Phone Number : " + smsMessage.PhoneNumber);
            Logger.LogWarning("SMS Text     : " + smsMessage.Text);

            return Task.CompletedTask;
        }
    }
}
