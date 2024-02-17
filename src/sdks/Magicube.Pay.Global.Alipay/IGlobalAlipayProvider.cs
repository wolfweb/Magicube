using Magicube.Pay.Global.Alipay.ViewModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using Magicube.Pay.Global.Alipay.Extensions;
using Magicube.Net;

namespace Magicube.Pay.Global.Alipay {
	public interface IGlobalAlipayProvider {
		Task<PayOutput> WebPay(PayInputViewModel input, GlobalAlipayOption option);
		bool PayNotifyHandler(Dictionary<string, string> dic, GlobalAlipayOption option);
	}

	public class GlobalAlipayProvider : IGlobalAlipayProvider {
		private readonly ILogger _logger;
		private readonly NotifyHelper _notifyHelper;
		public GlobalAlipayProvider(ILogger<GlobalAlipayProvider> logger, NotifyHelper notifyHelper) {
			_logger = logger;
			_notifyHelper = notifyHelper;
		}

		public Task<PayOutput> WebPay(PayInputViewModel input, GlobalAlipayOption option) {
			var sParaTemp = new SortedDictionary<string, string> {
				{"service"       , "create_forex_trade"},
				{"partner"       , option.Partner},
				{"_input_charset", option.CharSet.ToUpper()},
				{"sign_type"     , option.SignType},
				{"notify_url"    , input.NotifyUrl ?? option.Notify},
				{"return_url"    , input.ReturnUrl ?? option.ReturnUrl},
				{"currency"      , input.Currency ?? option.Currency},
				{"out_trade_no"  , input.TradeNo ?? Guid.NewGuid().ToString("N")},
				{"subject"       , input.Subject},
				{"product_code"  , "NEW_OVERSEAS_SELLER"},
			};
			if (!string.IsNullOrWhiteSpace(input.Body)) {
				//商品信息，不支持特殊字符。格式：[{"goods_name":"名称1","quantity":"数量1"},{"goods_name":"名称2","quantity":"数量2"}]。
				if (!input.Body.StartsWith("[")) {
					input.Body = JsonConvert.SerializeObject(new List<object>() {
						new {
							goods_name = input.Body,
							quantity = 1
						}
					});
				}
				sParaTemp.Add("body", input.Body);
			}
			if (input.RmbFee > 0) {
				sParaTemp.Add("rmb_fee", input.RmbFee.ToString());
			} else {
				sParaTemp.Add("total_fee", input.TotalFee.ToString());
			}

			if (!string.IsNullOrWhiteSpace(input.TimeoutRule)) {
				sParaTemp.Add("timeout_rule", input.TimeoutRule);
			}

			if (!string.IsNullOrWhiteSpace(input.AuthToken)) {
				sParaTemp.Add("auth_token", input.AuthToken);
			}

			if (!string.IsNullOrWhiteSpace(input.Supplier)) {
				sParaTemp.Add("supplier", input.Supplier);
			}

			if (!string.IsNullOrWhiteSpace(input.SecondaryMerchantId)) {
				sParaTemp.Add("secondary_merchant_id", input.SecondaryMerchantId);
			}

			if (!string.IsNullOrWhiteSpace(input.SecondaryMerchantName)) {
				sParaTemp.Add("secondary_merchant_name", input.SecondaryMerchantName);
			}

			if (!string.IsNullOrWhiteSpace(input.SecondaryMerchantIndustry)) {
				sParaTemp.Add("secondary_merchant_industry", input.SecondaryMerchantIndustry);
			}

			if (input.OrderGmtCreate.HasValue) {
				sParaTemp.Add("order_gmt_create", input.OrderGmtCreate.Value.ToString("yyyy-MM-dd hh:mm:ss"));
			}

			if (input.OrderValidTime.HasValue && input.OrderValidTime > 0) {
				sParaTemp.Add("order_valid_time", input.OrderValidTime.Value.ToString());
			}

			#region 设置分账信息
			if (input.SplitFundInfo != null && input.SplitFundInfo.Count > 0) {
				foreach (var splitFundInfoDto in input.SplitFundInfo) {
					if (input.RmbFee > 0) {
						splitFundInfoDto.Currency = "CNY";
					} else {
						splitFundInfoDto.Currency = input.Currency ?? option.Currency;
					}
				}
				//分账信息
				sParaTemp.Add("split_fund_info", Newtonsoft.Json.JsonConvert.SerializeObject(input.SplitFundInfo));
			} else if (option.SplitFundInfo != null && option.SplitFundInfo.Count > 0) {
				input.SplitFundInfo = new List<SplitFundInfoViewModel>();
				foreach (var splitFundInfo in option.SplitFundInfo) {
					var splitFundInfoDto = new SplitFundInfoViewModel() {
						Desc = splitFundInfo.Desc,
						TransIn = splitFundInfo.TransIn
					};
					if (input.RmbFee > 0) {
						splitFundInfoDto.Currency = "CNY";
					} else {
						splitFundInfoDto.Currency = input.Currency ?? option.Currency;
					}

					if (splitFundInfo.AmountRate > 0) {
						var amount = input.TotalFee > 0 ? input.TotalFee : input.RmbFee;
						//日元取整数,其他的保留两位小数
						splitFundInfoDto.Amount = splitFundInfoDto.Currency == "JPY" ? decimal.Floor(splitFundInfo.AmountRate * amount) : decimal.Round(splitFundInfo.AmountRate * amount, 2);
					}
				}
				//分账信息
				sParaTemp.Add("split_fund_info", Newtonsoft.Json.JsonConvert.SerializeObject(input.SplitFundInfo));
			}
			#endregion

			//过滤签名参数数组
			sParaTemp.FilterPara();
			var dic = sParaTemp.BuildRequestPara(option);
			_logger.LogDebug("支付参数：" + JsonConvert.SerializeObject(dic));
			var html = dic.GetHtmlSubmitForm(option.Gatewayurl, option.CharSet);
			return Task.FromResult(new PayOutput {
				FormHtml = html,
				Parameters = dic
			});

		}

		public bool PayNotifyHandler(Dictionary<string, string> dic, GlobalAlipayOption option) {
			try {
				var sArray = new SortedDictionary<string, string>();
				foreach (var item in dic) {
					sArray.Add(item.Key, item.Value);
				}

				var verifyResult = _notifyHelper.Verify(sArray, dic["notify_id"], dic["sign"], option);

				return verifyResult;
			} catch (Exception ex) {
				_logger.LogError("Error: {ex}", ex);
				return false;
			}
		}
	}
}