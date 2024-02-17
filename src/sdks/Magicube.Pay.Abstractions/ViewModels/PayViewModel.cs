using System.ComponentModel.DataAnnotations;

namespace Magicube.Pay.Abstractions.ViewModels {
    public class PayViewModel {
        public PayChannels PayChannel { get; set; }

        public string Key             { get; set; }

        [MaxLength(128)]
        public string Body            { get; set; }

        [Required]
        [MaxLength(256)]
        public string Subject         { get; set; }

        [Required]
        [Range(0.01, 100000000)]
        public decimal TotalAmount    { get; set; }

        [MaxLength(500)]
        [Display(Name = "自定义数据")]
        public string CustomData      { get; set; }

        [MaxLength(50)]
        [Display(Name = "交易单号")]
        public string OutTradeNo      { get; set; }

        [MaxLength(50)]
        [Display(Name = "OpenId")]
        public string OpenId          { get; set; }
    }
}