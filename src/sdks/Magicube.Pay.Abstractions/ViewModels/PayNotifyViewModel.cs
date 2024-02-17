namespace Magicube.Pay.Abstractions.ViewModels {
    public class PayNotifyViewModel {
        /// <summary>
        /// 租户Id（选填）
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// 提供程序简称，比如wechat、alipay
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// 当前请求
        /// </summary>
        //public HttpRequest Request { get; set; }
    }
}