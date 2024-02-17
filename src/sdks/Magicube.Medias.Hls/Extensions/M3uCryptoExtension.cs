using Magicube.Core;
using Magicube.Core.Encrypts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Magicube.Medias.Hls {
    internal static class M3uCryptoExtension {
        static readonly Dictionary<string, (int, int)> KeyGroup = new() { { "AES-128", (16, 24) }, { "AES-192", (24, 32) }, { "AES-256", (32, 44) } };

        public static byte[] HmacSha256(this Stream memory, byte[] key) {
            using var hmac = new HmacSha256CryptoEncryptProvider(key);
            return hmac.Encrypt(memory);
        }

        public static byte[] HmacSha256(this byte[] memory, byte[] key) {
            using var hmac = new HmacSha256CryptoEncryptProvider(key);
            return hmac.Encrypt(memory);
        }

        public static byte[] AesEncrypt(this byte[] content, byte[] aesKey, byte[] aesIv) {
            using var _aes = new AesSymmetricCryptoProvider(new SymmetricCryptoOptions { 
                PaddingMode        = PaddingMode.PKCS7,
                CipherMode         = CipherMode.CBC,
                SymmetricCryptoKey = aesKey.ToBase64(),
                SymmetricCryptoIv  = (aesIv ?? new byte[16]).ToBase64()
            });

            return _aes.Encrypt(content);
        }

        public static Stream AesEncrypt(this Stream memory, byte[] aesKey, byte[] aesIv) {
            using var _aes = new AesSymmetricCryptoProvider(new SymmetricCryptoOptions {
                PaddingMode        = PaddingMode.PKCS7,
                CipherMode         = CipherMode.CBC,
                SymmetricCryptoKey = aesKey.ToBase64(),
                SymmetricCryptoIv  = (aesIv ?? new byte[16]).ToBase64()
            });

            return new MemoryStream(_aes.Encrypt(memory));
        }

        public static byte[] AesDecrypt(this byte[] content, byte[] aesKey, byte[] aesIv) {
            using var _aes = new AesSymmetricCryptoProvider(new SymmetricCryptoOptions {
                PaddingMode        = PaddingMode.PKCS7,
                CipherMode         = CipherMode.CBC,
                SymmetricCryptoKey = aesKey.ToBase64(),
                SymmetricCryptoIv  = (aesIv ?? new byte[16]).ToBase64()
            });

            return _aes.Decrypt(content);
        }

        public static Stream AesDecrypt(this Stream memory, byte[] aesKey, byte[] aesIv) {
            using var aesAlg = new AesSymmetricCryptoProvider(new SymmetricCryptoOptions { 
                PaddingMode        = PaddingMode.PKCS7,
                CipherMode         = CipherMode.CBC,
                SymmetricCryptoKey = aesKey.ToBase64(),
                SymmetricCryptoIv  = (aesIv ?? new byte[16]).ToBase64()
            });

            return new MemoryStream(aesAlg.Decrypt(memory));
        }

        public static byte[] TryParseKey(this byte[] data, string method) {
            string tmpMethod = string.IsNullOrWhiteSpace(method) ? "AES-128" : method.ToUpper(CultureInfo.CurrentCulture).Trim();
            if (KeyGroup.TryGetValue(tmpMethod, out (int, int) tmpKey)) {
                if (data.Length == tmpKey.Item1)
                    return data;
                else if (data.Length == tmpKey.Item2) {
                    var stringdata = Encoding.UTF8.GetString(data);
                    return Convert.FromBase64String(stringdata);
                }
            }
            throw new InvalidCastException($"无法解析的密钥,请确定是否为AES-128,AES-192,AES-256");
        }
    }
}