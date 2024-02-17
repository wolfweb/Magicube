using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Magicube.Pay.Global.Alipay {
	public class GlobalAlipayOption {
		/// <summary>
		///     Gets or sets the Key
		///     MD5密钥，安全检验码，由数字和字母组成的32位字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		///     Gets or sets the Partner
		///     合作商户uid(合作身份者ID，签约账号，以2088开头由16位纯数字组成的字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm)
		/// </summary>
		public string Partner { get; set; }

		/// <summary>
		///     Gets or sets the Gatewayurl
		///     支付宝网关
		/// </summary>
		public string Gatewayurl { get; set; }

		/// <summary>
		///     Gets or sets the SignType
		///     签名方式（默认值：MD5）
		/// </summary>
		public string SignType { get; set; }

		/// <summary>
		///     Gets or sets the CharSet
		///     编码格式（默认值：UTF-8）
		/// </summary>
		public string CharSet { get; set; }

		/// <summary>
		///     Gets or sets the Notify
		///     服务器异步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数,必须外网可以正常访问
		/// </summary>
		public string Notify { get; set; }

		/// <summary>
		///     页面跳转同步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数，必须外网可以正常访问
		/// </summary>
		public string ReturnUrl { get; set; }

		/// <summary>
		///     结算币种
		/// </summary>
		public string Currency { get; set; }

		/// <summary>
		/// 分账信息
		/// </summary>
		public List<SplitFundSettingInfoDto> SplitFundInfo { get; set; }
	}

	public class SplitFundSettingInfoDto {
		/// <summary>
		/// 接受分账资金的支付宝账户ID。 以2088开头的纯16位数字。
		/// </summary>
		[JsonProperty("transIn")]
		[Required]
		[MaxLength(16)]
		public string TransIn { get; set; }

		/// <summary>
		/// 分账比率
		/// </summary>
		[JsonProperty("amountRate")]
		[Required]
		[Range(0.1, 1)]
		public decimal AmountRate { get; set; }

		/// <summary>
		/// 分账描述信息。
		/// </summary>
		[JsonProperty("desc")]
		[MaxLength(200)]
		public string Desc { get; set; }
	}
}