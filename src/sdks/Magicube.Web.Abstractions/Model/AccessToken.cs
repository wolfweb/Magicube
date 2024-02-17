using System;

namespace Magicube.Web {
    public class AccessToken {
		public const int NonceSize = 16;
		public AccessToken(long id, int expireMinutes = 15) {
			Id      = id;
			Exprire = DateTimeOffset.Now.AddMinutes(expireMinutes).ToUnixTimeSeconds();
			Nonce   = Guid.NewGuid().ToString("n").Substring(0, NonceSize);
		}
		public long   Id      { get; }
		public long   Exprire { get; set; }
		public string Nonce   { get; set; }
	}
}
