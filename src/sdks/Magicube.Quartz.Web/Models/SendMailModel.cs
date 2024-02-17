using System.ComponentModel.DataAnnotations;

namespace Magicube.Quartz.Web.Models {
    public class SendMailModel {
        [Display(Name="收件人")]
        public string Receiver { get; set; }
        [Display(Name = "主题")]
        public string Title    { get; set; }
        [Display(Name = "内容")]
        public string Body     { get; set; }
    }
}
