using System.Collections.Generic;

namespace Magicube.Pay.Global.Alipay {
	public class PayOutput {
		/// <summary>
		/// 参数列表
		/// </summary>
		public Dictionary<string, string> Parameters { get; set; }

		/// <summary>
		/// 表单Html
		/// </summary>
		public string FormHtml { get; set; }
	}
}