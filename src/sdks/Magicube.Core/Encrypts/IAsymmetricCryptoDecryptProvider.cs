using Magicube.Core.Runtime;
using Magicube.Core.Runtime.Attributes;
using Microsoft.Extensions.Options;
using RSAExtensions;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;

namespace Magicube.Core.Encrypts {
    public interface IAsymmetricCryptoDecryptProvider : ICryptoDecryptProvider {
        string PublicKey         { get; set; }
        string PrivateKey        { get; set; }
        bool   GenerateSecretKey { get; set; }
    }

    public class RasCryptoEncryptProvider : IAsymmetricCryptoDecryptProvider, IRuntimeMetadata<ICryptoProvider> {
		private const string Pattern = "-{5}BEGIN [A-Z0-9 ]+-{5}([a-zA-Z0-9=+\\/\\n\\r]+)-{5}END [A-Z0-9 ]+-{5}";

		public string Name              => "Ras";
        public string Group             => "非对称ras加密";
        public string PublicKey         { get; set; }
        public string PrivateKey        { get; set; }
        public bool   GenerateSecretKey { get; set; }

        private static int _initialized = 0;

        private readonly RasCryptoOptions _rasCryptoOptions;
        private readonly RSA _rsa;

        public RasCryptoEncryptProvider(IOptionsMonitor<RasCryptoOptions> options) {
            _rsa              = new RSACryptoServiceProvider(2048);
            _rasCryptoOptions = options.CurrentValue;
            if (options.CurrentValue != null) {
                GenerateSecretKey = options.CurrentValue.GenerateSecretKey;

                if (options.CurrentValue.GenerateSecretKey && options.CurrentValue.PublicKey.IsNullOrEmpty() && options.CurrentValue.PrivateKey.IsNullOrEmpty()) {
                    EnsureProvider();
                }else if(!options.CurrentValue.PublicKey.IsNullOrEmpty() && !options.CurrentValue.PrivateKey.IsNullOrEmpty()) {
                    PublicKey  = options.CurrentValue.PublicKey;
                    PrivateKey = options.CurrentValue.PrivateKey;
                    EnsureProvider();
                } else {
                    throw new MagicubeException(10000, "rsa crypto should auto generate secret key or given public key and private key");
                }
            }
        }

        [RuntimeMethod]
        public string Decrypt(string data, string charSet = "utf-8") {
            return Decrypt(Convert.FromBase64String(data)).ToString(charSet);
        }

        public byte[] Decrypt(byte[] data) {
            EnsureProvider();
            return _rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
        }

        public byte[] Decrypt(Stream data) {
            using(var mem = new MemoryStream()) {
                data.CopyTo(mem);
                return Decrypt(mem.ToArray());
            }
        }

        [RuntimeMethod]
        public string Encrypt(string data, string charSet = "utf-8") {
            return Convert.ToBase64String(Encrypt(data.ToByte(charSet)));
        }

        public byte[] Encrypt(byte[] data) {
            EnsureProvider();
            return _rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
        }

        public byte[] Encrypt(Stream data) {
            using(var stream = new MemoryStream()) {
                data.CopyTo(stream);
                return Encrypt(stream.ToArray());
            }
        }

        private void EnsureProvider() {
            if (_initialized > 0) return;

            if (GenerateSecretKey) {
				PublicKey  = _rsa.ExportPublicKey(RSAKeyType.Pkcs8);
                PrivateKey = _rsa.ExportPrivateKey(RSAKeyType.Pkcs8);
			} else {
				if(PublicKey.IsNullOrEmpty() || PrivateKey.IsNullOrEmpty()) throw new InvalidDataException("invalid public and private key for this decrypt provider!");
                _rsa.ImportPublicKey(_rasCryptoOptions.Type, PublicKey, IsPem(PublicKey));
                _rsa.ImportPrivateKey(_rasCryptoOptions.Type, PrivateKey, IsPem(PrivateKey));
			}

            Interlocked.Increment(ref _initialized);
        }

        private bool IsPem(string data) {
			return Regex.IsMatch(data, Pattern, RegexOptions.Compiled);
		}

		public void Dispose() {
            _rsa.Dispose();
        }
    }
}
