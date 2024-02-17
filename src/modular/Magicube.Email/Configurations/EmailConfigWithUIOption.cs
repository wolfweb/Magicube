using Magicube.Core;
using Magicube.Net.Email;
using Magicube.Web.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Magicube.Email.Configurations {
    public class EmailConfigWithUIOption : ShouldConfigWithUIOption {
        private readonly MailOption _mailOption;
        public EmailConfigWithUIOption(
            IOptionsMonitor<MailOption> options, 
            IHttpContextAccessor httpContextAccessor
            ) : base(httpContextAccessor) {
            _mailOption = options.CurrentValue;
        }

        public override RouteValueDictionary Routes => new RouteValueDictionary {
            ["area"]       = "Magicube.Email",
            ["action"]     = "Index",
            ["controller"] = "Admin",
        };

        public MailOption Option => _mailOption;

        protected override bool OnConfigure() {
            var configProvider = GetService<IMagicubeConfigProvider<MailOption>>();
            var option         = configProvider.GetSetting();
            if (option == null) return false;

            _mailOption.CC          = option.CC;
            _mailOption.Port        = option.Port;
            _mailOption.Server      = option.Server;
            _mailOption.Password    = option.Password;
            _mailOption.UserName    = option.UserName;
            _mailOption.SenderName  = option.SenderName;
            _mailOption.SenderEmail = option.SenderEmail;

            return true;
        }
    }
}
