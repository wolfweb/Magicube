using Magicube.Pay.Abstractions.Entities;

namespace Magicube.Pay.Wechatpay.Entities {
    public class WechatpayEntity : PayEntity {
        /// <summary>
        /// 应用密钥
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MchId { get; set; }

        /// <summary>
        /// 子商户应用号
        /// </summary>
        public string SubAppId { get; set; }

        /// <summary>
        /// 子商户号
        /// </summary>
        public string SubMchId { get; set; }


    }
}
