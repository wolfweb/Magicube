using Magicube.Email.Configurations;
using Magicube.Net.Email;

namespace Magicube.Email.Services {
    class EmailConfigResolve : IEmailConfigResolve {
        private readonly EmailConfigWithUIOption _emailConfigWithUIOption;

        public EmailConfigResolve(EmailConfigWithUIOption emailConfigWithUIOption) {
            if (!emailConfigWithUIOption.TryConfigure()) {
                emailConfigWithUIOption.DoRedirect();
            }

            _emailConfigWithUIOption = emailConfigWithUIOption;
        }

        public MailOption Option => _emailConfigWithUIOption.Option;
    }
}
