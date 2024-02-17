using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Magicube.Pay.Global.Alipay.Extensions {
	public static class IDictionaryExtension {
		public static Dictionary<string, string> FilterPara(this SortedDictionary<string, string> dicArrayPre) {
			var dicArray = new Dictionary<string, string>();
			foreach (var temp in dicArrayPre)
				if (temp.Key.ToLower() != "sign" && temp.Key.ToLower() != "sign_type" &&
					!string.IsNullOrEmpty(temp.Value))
					dicArray.Add(temp.Key, temp.Value);

			return dicArray;
		}

		public static Dictionary<string, string> BuildRequestPara(this SortedDictionary<string, string> sParaTemp, GlobalAlipayOption alipaySettings) {
			//待签名请求参数数组
			//签名结果
			var mysign = "";

			//过滤签名参数数组
			var sPara = FilterPara(sParaTemp);

			//获得签名结果
			mysign = BuildRequestMysign(sPara, alipaySettings);

			//签名结果与签名方式加入请求提交参数组中
			sPara.Add("sign", mysign);
			sPara.Add("sign_type", alipaySettings.SignType);

			return sPara;
		}

		public static string CreateLinkString(this Dictionary<string, string> dicArray) {
			var prestr = new StringBuilder();
			foreach (var temp in dicArray) prestr.Append(temp.Key + "=" + temp.Value + "&");

			//去掉最後一個&字符
			var nLen = prestr.Length;
			prestr.Remove(nLen - 1, 1);

			return prestr.ToString();
		}

		public static string GetHtmlSubmitForm(this Dictionary<string, string> dicPara, string gatewayurl,
			string charSet) {
			var sbHtml = new StringBuilder();

			sbHtml.Append("<form id='alipaysubmit' name='alipaysubmit' action='" + gatewayurl + "' _input_charset=" +
						  charSet + "' method='get'>");

			foreach (var temp in dicPara)
				sbHtml.Append("<input type='hidden' name='" + temp.Key + "' value='" + temp.Value + "'/>");

			//submit按钮控件请不要含有name属性
			sbHtml.Append("<input type='submit' value='支付宝' style='display:none;'></form>");

			sbHtml.Append("<script>document.forms['alipaysubmit'].submit();</script>");

			return sbHtml.ToString();
		}

		public static string Sign(string prestr, string key, string inputCharset) {
			var sb = new StringBuilder(32);

			prestr = prestr + key;

			MD5 md5 = MD5.Create();
			var t = md5.ComputeHash(Encoding.GetEncoding(inputCharset).GetBytes(prestr));
			foreach (var t1 in t) sb.Append(t1.ToString("x").PadLeft(2, '0'));

			return sb.ToString();
		}

		private static string BuildRequestMysign(this Dictionary<string, string> sPara, GlobalAlipayOption alipaySettings) {
			//把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
			var prestr = sPara.CreateLinkString();

			//把最终的字符串签名，获得签名结果
			var mysign = "";
			switch (alipaySettings.SignType) {
				case "MD5":
					mysign = Sign(prestr, alipaySettings.Key, alipaySettings.CharSet);
					break;
				default:
					mysign = "";
					break;
			}

			return mysign;
		}
	}
}
