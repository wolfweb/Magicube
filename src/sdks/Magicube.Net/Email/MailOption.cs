using System.ComponentModel.DataAnnotations;

namespace Magicube.Net.Email {
    public class MailOption {
        [Display(Name = "端口")]
        public int      Port                  { get; set; } = 25;

        [Required]
        [Display(Name = "服务器地址")]
        public string   Server                { get; set; }

        [Display(Name = "账号")]
        public string   UserName              { get; set; }

        [Display(Name = "密码")]
        public string   Password              { get; set; }

        [Required]
        [Display(Name = "发件人地址")]
        public string   SenderEmail           { get; set; }

        [Display(Name = "发件人名称")]
        public string   SenderName            { get; set; }

        [Display(Name = "抄送人地址")]
        public string[] CC                    { get; set; }
    }
}
