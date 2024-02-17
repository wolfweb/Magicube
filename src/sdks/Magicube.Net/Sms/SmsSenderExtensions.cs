using Magicube.Core;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Magicube.Net.Sms {
    public static class SmsSenderExtensions {
        public static Task SendAsync([NotNull] this ISmsSender smsSender, [NotNull] string phoneNumber, [NotNull] string text) {
            smsSender.NotNull();
            return smsSender.SendAsync(new SmsMessage(phoneNumber, text));
        }
    }
}
