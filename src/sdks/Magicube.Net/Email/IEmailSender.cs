using Magicube.Core;
using MailKit.Security;
using MimeKit;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Magicube.Net.Email {
    public interface IEmailSender {
        string Sender { get; }
        Task SendAsync(string to, string subject, string body, bool isHtml = true);
        Task SendAsync(string from, string to, string subject, string body, bool isHtml = true);
        Task SendAsync(MailMessage mail);
    }

    public class EmailSender : IEmailSender {
        private readonly MailOption _mailOption;

        public EmailSender(IEmailConfigResolve emailConfgiResolve) {
            _mailOption = emailConfgiResolve.Option;
        }

        public string Sender => _mailOption.SenderEmail;

        public async Task SendAsync(string to, string subject, string body, bool isHtml = true) {
            var message = new MailMessage {
                To         = { to },
                Subject    = subject,
                Body       = body,
                IsBodyHtml = isHtml
            };

            if( _mailOption.CC != null) {
                foreach (var item in _mailOption.CC) {
                    message.CC.Add(item);
                }
            }

            await SendAsync(message);
        }

        public async Task SendAsync(string from, string to, string subject, string body, bool isHtml = true) {
            await SendAsync(new MailMessage(from, to, subject, body) { IsBodyHtml = isHtml });
        }

        public async Task SendAsync(MailMessage mail) {
            ParseMail(mail);

            await SendEmailAsync(mail);
        }

        public virtual async Task SendEmailAsync(MailMessage mail) {
            using (var client = await BuildClient()) {
                var message = MimeMessage.CreateFromMailMessage(mail);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        private async Task<SmtpClient> BuildClient() {
            var client = new SmtpClient();

            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            await client.ConnectAsync(_mailOption.Server, _mailOption.Port, SecureSocketOptions.Auto);

            await client.AuthenticateAsync(_mailOption.UserName, _mailOption.Password);
            return client;
        }

        private void ParseMail(MailMessage mail) {
            if (mail.From == null || mail.From.Address.IsNullOrEmpty()) {
                mail.From = new MailAddress(_mailOption.SenderEmail, _mailOption.SenderName);
            }

            if (mail.HeadersEncoding == null) {
                mail.HeadersEncoding = Encoding.UTF8;
            }

            if (mail.SubjectEncoding == null) {
                mail.SubjectEncoding = Encoding.UTF8;
            }

            if (mail.BodyEncoding == null) {
                mail.BodyEncoding = Encoding.UTF8;
            }
        }
    }
}
