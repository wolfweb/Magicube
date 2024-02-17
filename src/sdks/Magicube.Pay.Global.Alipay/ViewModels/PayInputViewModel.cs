using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicube.Pay.Global.Alipay.ViewModels {
	public class PayInputViewModel {
		/// <summary>
		///     对一笔交易的具体描述信息。如果是多种商品，请将商品描述字符串累加传给body。
		/// </summary>
		[MaxLength(400)]
		public string Body { get; set; }

		/// <summary>
		///     商品的标题/交易标题/订单标题/订单关键字等。
		/// </summary>
		[Required]
		[MaxLength(255)]
		public string Subject { get; set; }

		/// <summary>
		///     商户网站唯一订单号（为空则自动生成）
		/// </summary>
		[MaxLength(64)]
		public string TradeNo { get; set; }

		/// <summary>
		///     结算币种
		/// </summary>
		[MaxLength(10)]
		public string Currency { get; set; }

		/// <summary>
		///     商品的外币金额，范围是0.01～1000000.00.
		/// </summary>
		[Range(0.01, 1000000)]
		public decimal TotalFee { get; set; }


		/// <summary>
		///     范围为0.01～1000000.00,如果商户网站使用人民币进行标价就是用这个参数来替换total_fee参数，rmb_fee和total_fee不能同时使用
		/// </summary>
		[Range(0.01, 1000000)]
		public decimal RmbFee { get; set; }


		/// <summary>
		///     默认12小时，最大15天。此为买家登陆到完成支付的有效时间
		/// </summary>
		[MaxLength(10)]
		public string TimeoutRule { get; set; }

		/// <summary>
		///     快捷登录返回的安全令牌。快捷登录的需要传。
		/// </summary>
		[MaxLength(40)]
		public string AuthToken { get; set; }

		/// <summary>
		///     YYYY-MM-DD HH:MM:SS 这里请使用北京时间以便于和支付宝系统时间匹配，此参数必须要和order_valid_time参数一起使用，控制从跳转到买家登陆的有效时间
		/// </summary>
		public DateTime? OrderGmtCreate { get; set; }


		/// <summary>
		///     最大值为2592000，单位为秒，此参数必须要和order_gmt_create参数一起使用，控制从跳转到买家登陆的有效时间
		/// </summary>
		public int? OrderValidTime { get; set; }

		/// <summary>
		///     显示供货商名字
		/// </summary>
		[MaxLength(16)]
		public string Supplier { get; set; }

		/// <summary>
		///     由支付机构给二级商户分配的唯一ID
		/// </summary>
		[MaxLength(64)]
		public string SecondaryMerchantId { get; set; }

		/// <summary>
		///     由支付机构给二级商户分配的唯一名称
		/// </summary>
		[MaxLength(128)]
		public string SecondaryMerchantName { get; set; }

		/// <summary>
		///     支付宝分配的二级商户的行业代码，如公共饮食行业、餐馆：5812,百货公司：5311,住房-旅馆：7011,出租车：4121。
		/// </summary>
		[MaxLength(4)]
		public string SecondaryMerchantIndustry { get; set; }

		/// <summary>
		///     支付宝服务器主动通知商户服务器里指定的页面http/https路径。建议商户使用https
		/// </summary>
		[MaxLength(256)]
		[Required]
		public string NotifyUrl { get; set; }


		/// <summary>
		///     页面跳转同步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数，必须外网可以正常访问
		/// </summary>
		[MaxLength(256)]
		public string ReturnUrl { get; set; }

		/// <summary>
		/// 分账信息
		/// </summary>
		public List<SplitFundInfoViewModel> SplitFundInfo { get; set; }
	}

	public class SplitFundInfoViewModel {
		/// <summary>
		/// 接受分账资金的支付宝账户ID。 以2088开头的纯16位数字。
		/// </summary>
		[JsonProperty("transIn")]
		[Required]
		[MaxLength(16)]
		public string TransIn { get; set; }

		/// <summary>
		/// 分账的金额。格式必须符合相应币种的要求，比如：日元为整数，人民币最多２位小数。当分账币种是CNY时，此金额代表的是人民币；如果分账币种是外币时，此金额则是外币。但分账商户实际收到的金额始终是人民币，如果分账明细中是外币，分账商户得到的人民币实际是通过汇率进行计算得到的。数值（小数点后最多2位）
		/// </summary>
		[JsonProperty("amount")]
		[Required]
		[Range(0.1, 10000000)]
		public decimal Amount { get; set; }

		/// <summary>
		/// 分账币种。如果total_fee不为空，则分账币种必须是外币，且与结算币种一致；如果rmb_fee不为空，则分账币种必须是人民币。人民币填写“CNY”，外币请参见“币种列表”。
		/// </summary>
		[JsonProperty("currency")]
		[Required]
		public string Currency { get; set; } = "USD";

		/// <summary>
		/// 分账描述信息。
		/// </summary>
		[JsonProperty("desc")]
		public string Desc { get; set; }
	}
}
