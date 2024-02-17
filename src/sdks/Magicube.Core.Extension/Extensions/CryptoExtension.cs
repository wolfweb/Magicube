using Magicube.Core.Encrypts;

namespace Magicube.Core {
#if !DEBUG
using System.Diagnostics;
    [DebuggerStepThrough]
#endif
    public static class CryptoExtension {
        public static string Encrypt(this HmacMd5CryptoEncryptProvider hmacMd5, string key, string data) {
            hmacMd5.Hmac.Key = key.ToByte();
            return hmacMd5.Encrypt(data);
        }

        public static string Encrypt(this HmacSha1CryptoEncryptProvider hmacSha1, string key, string data) {
            hmacSha1.Hmac.Key = key.ToByte();
            return hmacSha1.Encrypt(data);
        }

        public static string Encrypt(this HmacSha256CryptoEncryptProvider hmacSha256, string key, string data) {
            hmacSha256.Hmac.Key = key.ToByte();
            return hmacSha256.Encrypt(data);
        }

        public static string Encrypt(this HmacSha512CryptoEncryptProvider hmacSha512, string key, string data) {
            hmacSha512.Hmac.Key = key.ToByte();
            return hmacSha512.Encrypt(data);
        }
    }
}
