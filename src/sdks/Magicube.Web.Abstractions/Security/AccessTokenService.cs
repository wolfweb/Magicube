using Magicube.Core.Encrypts;
using Magicube.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magicube.Web.Security {
    public class AccessTokenService : IRuntimeMetadata {
		public string GenerateAccessToken(AccessToken accessToken) {
			byte[] userId    = BitConverter.GetBytes(accessToken.Id),
			expire           = BitConverter.GetBytes(accessToken.Exprire),
			nonce            = Encoding.UTF8.GetBytes(accessToken.Nonce);
			int count        = userId.Length + expire.Length + nonce.Length, pos = 0;
			var userDatas    = userId.Concat(expire).ToList();
			for (var i = 0; i < count && pos < nonce.Length; i++) {
				if (pos < nonce.Length && i % 2 == 0) {
					userDatas.Insert(i, nonce[pos++]);
				}
			}

			return Base64UrlEncoder.Encode(userDatas.ToArray());
		}

		public AccessToken DecodeAccessToken(string token) {
			try {
				var bytes = Base64UrlEncoder.DecodeBytes(token);
				List<byte> userDatas = new List<byte>(), nonce = new List<byte>();
				var pos = 0;
				for (var i = 0; i < bytes.Length; i++) {
					if (pos < AccessToken.NonceSize && i % 2 == 0) {
						pos++;
						nonce.Add(bytes[i]);
						continue;
					}
					userDatas.Add(bytes[i]);
				}

				bytes = userDatas.ToArray();
				return new AccessToken(BitConverter.ToInt64(bytes, 0)) {
					Exprire = BitConverter.ToInt64(bytes, 8),
					Nonce = Encoding.UTF8.GetString(nonce.ToArray())
				};
			} catch{
				return null;
			}
		}
	}
}
