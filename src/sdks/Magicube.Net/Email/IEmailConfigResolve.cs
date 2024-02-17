using Microsoft.Extensions.Options;

namespace Magicube.Net.Email {
    public interface IEmailConfigResolve {
        MailOption Option { get; }
    }

    public class EmailConfigResolve : IEmailConfigResolve { 
        private readonly MailOption _option;
        public EmailConfigResolve(IOptionsMonitor<MailOption> options) {
            _option = options.CurrentValue;
        }

        public MailOption Option => _option;
    }
}
