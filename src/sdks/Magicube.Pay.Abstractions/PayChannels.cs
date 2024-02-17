namespace Magicube.Pay.Abstractions {
    public class PayChannels {
        public int ChannelKey { get; }

        public PayChannels(int channelKey) {
            ChannelKey = channelKey;
        }

        /// <summary>
        /// 微信支付
        /// </summary>
        public static PayChannels WeChatPay = 0;

        /// <summary>
        /// 支付宝APP支付
        /// </summary>
        public static PayChannels AliAppPay = 1;

        /// <summary>
        /// 余额支付
        /// </summary>
        public static PayChannels BalancePay = 2;

        /// <summary>
        /// 国际支付宝
        /// </summary>
        public static PayChannels GlobalAlipay = 3;

        /// <summary>
        /// 微信小程序支付
        /// </summary>
        public static PayChannels WeChatMiniProgram = 4;

        /// <summary>
        /// 通联微信小程序
        /// </summary>
        public static PayChannels AllinWeChatMiniPay = 5;

        /// <summary>
        /// 微信APP支付
        /// </summary>
        public static PayChannels WeChatAppPay = 6;

        /// <summary>
        /// 通联JSAPI
        /// </summary>
        public static PayChannels AllinJsApiPay = 7;
        /// <summary>
        /// 农业银行
        /// </summary>
        public static PayChannels AbcPay = 8;
        /// <summary>
        /// 工商银行
        /// </summary>
        public static PayChannels IcbcPay = 9;

        public static implicit operator PayChannels(int channelKey) => new PayChannels(channelKey);
    }
}