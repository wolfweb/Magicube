using System.Collections.Generic;
using System;
using Magicube.Pay.Global.Alipay.Extensions;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Magicube.Net;
using Magicube.Core.Encrypts;

namespace Magicube.Pay.Global.Alipay {
	public class NotifyHelper {
		//支付宝消息验证地址
		private const string Https_veryfy_url = "https://mapi.alipay.com/gateway.do?service=notify_verify&";

		private readonly Md5CryptoEncryptProvider _md5CryptoEncryptProvider;
		private readonly ILogger _logger;
		private readonly Curl _curl;

		public NotifyHelper(ILogger logger, Curl curl, Md5CryptoEncryptProvider md5CryptoEncryptProvider) {
			_logger                   = logger;
			_curl                     = curl;
			_md5CryptoEncryptProvider = md5CryptoEncryptProvider;
		}

		/// <summary>
		///     验证消息是否是支付宝发出的合法消息
		/// </summary>
		/// <param name="inputPara">通知返回参数数组</param>
		/// <param name="notifyId">通知验证ID</param>
		/// <param name="sign">支付宝生成的签名结果</param>
		/// <param name="option"></param>
		/// <returns>验证结果</returns>
		public bool Verify(SortedDictionary<string, string> inputPara, string notifyId, string sign, GlobalAlipayOption option) {
			//获取返回时的签名验证结果
			var isSign = GetSignVeryfy(inputPara, sign, option);
			//获取是否是支付宝服务器发来的请求的验证结果
			var responseTxt = "false";
			if (!string.IsNullOrEmpty(notifyId)) responseTxt = GetResponseTxt(notifyId, option);

			//写日志记录（若要调试，请取消下面两行注释）
			var sWord = "responseTxt=" + responseTxt + "\n isSign=" + isSign + "\n 返回的参数：" + GetPreSignStr(inputPara) +
						"\n ";

			_logger.LogDebug("Debug: {sWord}", sWord);

			//判断responsetTxt是否为true，isSign是否为true
			//responsetTxt的结果不是true，与服务器设置问题、合作身份者ID、notify_id一分钟失效有关
			//isSign不是true，与安全校验码、请求时的参数格式（如：带自定义参数等）、编码格式有关
			return responseTxt == "true" && isSign;
		}

		/// <summary>
		///     验证消息是否是支付宝发出的消息
		/// </summary>
		/// <param name="inputPara">通知返回参数数组</param>
		/// <param name="sign">支付宝生成的签名结果</param>
		/// <returns>验证结果</returns>
		public bool VerifyReturn(SortedDictionary<string, string> inputPara, string sign, GlobalAlipayOption option) {
			//获取返回时的签名验证结果
			var isSign = GetSignVeryfy(inputPara, sign, option);
			return isSign;
		}

		/// <summary>
		///     获取待签名字符串（调试用）
		/// </summary>
		/// <param name="inputPara">通知返回参数数组</param>
		/// <returns>待签名字符串</returns>
		private string GetPreSignStr(SortedDictionary<string, string> inputPara) {
			//过滤空值、sign与sign_type参数
			var sPara = inputPara.FilterPara();

			//获取待签名字符串
			var preSignStr = sPara.CreateLinkString();

			return preSignStr;
		}

		/// <summary>
		///     获取返回时的签名验证结果
		/// </summary>
		/// <param name="inputPara">通知返回参数数组</param>
		/// <param name="sign">对比的签名结果</param>
		/// <returns>签名验证结果</returns>
		private bool GetSignVeryfy(SortedDictionary<string, string> inputPara, string sign, GlobalAlipayOption option) {
			//过滤空值、sign与sign_type参数
			var sPara = inputPara.FilterPara();

			//获取待签名字符串
			var preSignStr = sPara.CreateLinkString();

			//获得签名验证结果
			var isSgin = false;
			if (string.IsNullOrEmpty(sign)) return false;
			switch (option.SignType) {
				case "MD5":
					isSgin = Verify_Sign(sign, preSignStr, option.Key, option.CharSet);
					break;
				default:
					break;
			}

			return isSgin;
		}

		/// <summary>
		///     获取是否是支付宝服务器发来的请求的验证结果
		/// </summary>
		/// <param name="notifyId">通知验证ID</param>
		/// <returns>验证结果</returns>
		private string GetResponseTxt(string notifyId, GlobalAlipayOption option) {
			var veryfyUrl = $"{Https_veryfy_url}partner={option.Partner}&notify_id={notifyId}";

			//获取远程服务器ATN结果，验证是否是支付宝服务器发来的请求
			var responseTxt = Get_Http(veryfyUrl, 120000);

			return responseTxt;
		}

		/// <summary>
		///     获取远程服务器ATN结果
		/// </summary>
		/// <param name="strUrl">指定URL路径地址</param>
		/// <param name="timeout">超时时间设置</param>
		/// <returns>服务器ATN结果</returns>
		private string Get_Http(string strUrl, int timeout) {
			string strResult;
			try {
				var myReq = WebRequest.Create(strUrl);
				myReq.Timeout = timeout;
				var res = myReq.GetResponse();
				using (var myStream = res.GetResponseStream()) {
					var sr = new StreamReader(myStream ?? throw new InvalidOperationException("获取远程结果为空"),
						Encoding.Default);
					var strBuilder = new StringBuilder();
					while (-1 != sr.Peek()) strBuilder.Append(sr.ReadLine());

					strResult = strBuilder.ToString();
				}
			} catch (Exception exp) {
				strResult = "错误：" + exp.Message;
			}

			return strResult;
		}

		private bool Verify_Sign(string sign, string prestr, string key, string charSet) {
			return sign == _md5CryptoEncryptProvider.Encrypt(prestr + key);
		}
	}
}