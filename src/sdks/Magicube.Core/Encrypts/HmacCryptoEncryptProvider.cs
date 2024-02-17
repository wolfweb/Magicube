using Magicube.Core.Runtime;
using Magicube.Core.Runtime.Attributes;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Magicube.Core.Encrypts {
    public abstract class HmacCryptoEncryptProvider : ICryptoEncryptProvider {
        public abstract string Name  { get; }
        public          string Group => "Hamc加密";

        protected static ConcurrentDictionary<string, HMAC> _hmacCryptos = new ConcurrentDictionary<string, HMAC>();

        public readonly HMAC Hmac;
        private readonly string _name; 
             
        public HmacCryptoEncryptProvider(string name) {
            _name = name;
            Hmac = _hmacCryptos.GetOrAdd(name, x => HMAC.Create(x));
        }

        [RuntimeMethod]
        public string Encrypt(string data, string charSet = "utf-8") {
            var datas  = data.ToByte(charSet);
            var result = Hmac.ComputeHash(datas);
            return result.Aggregate("", (current, t) => current + t.ToString("X").PadLeft(2, '0'));
        }

        public byte[] Encrypt(byte[] data) {
            return Hmac.ComputeHash(data);
        }

        public byte[] Encrypt(Stream data) {
            return Hmac.ComputeHash(data);
        }

        public void Dispose() {
            Hmac.Dispose();
            _hmacCryptos.TryRemove(_name,out _);
        }
    }

    public class HmacMd5CryptoEncryptProvider : HmacCryptoEncryptProvider, IRuntimeMetadata<ICryptoProvider> {
        public const    string Identity = "HmacMd5";
        public override string Name     => Identity;
        public HmacMd5CryptoEncryptProvider(IOptionsMonitor<HmacCryptoOptions> options) : base(Identity) {
            if (options.CurrentValue != null && options.CurrentValue.CryptoName == Name) {
                Hmac.Key = options.CurrentValue.HmacSecretKey.ToByte();
            }
        }

        public HmacMd5CryptoEncryptProvider(string key) : base(Identity) {
            Hmac.Key = key.ToByte();
        }

        public HmacMd5CryptoEncryptProvider(byte[] key) : base(Identity) {
            Hmac.Key = key;
        }
    }

    public class HmacSha1CryptoEncryptProvider : HmacCryptoEncryptProvider, IRuntimeMetadata<ICryptoProvider> {
        public const string Identity = "HmacSha1";
        public override string Name  => Identity;
        public HmacSha1CryptoEncryptProvider(IOptionsMonitor<HmacCryptoOptions> options) : base(Identity) {
            if (options.CurrentValue != null && options.CurrentValue.CryptoName == Name) {
                Hmac.Key = options.CurrentValue.HmacSecretKey.ToByte();
            }
        }

        public HmacSha1CryptoEncryptProvider(string key) : base(Identity) {
            Hmac.Key = key.ToByte();
        }

        public HmacSha1CryptoEncryptProvider(byte[] key) : base(Identity) {
            Hmac.Key = key;
        }
    }

    public class HmacSha256CryptoEncryptProvider : HmacCryptoEncryptProvider, IRuntimeMetadata<ICryptoProvider> {
        public const string Identity = "HmacSha256";
        public override string Name  => Identity;
        public HmacSha256CryptoEncryptProvider(IOptionsMonitor<HmacCryptoOptions> options) : base(Identity) {
            if (options.CurrentValue != null && options.CurrentValue.CryptoName == Name) {
                Hmac.Key = options.CurrentValue.HmacSecretKey.ToByte();
            }
        }

        public HmacSha256CryptoEncryptProvider(string key) : base(Identity) {
            Hmac.Key = key.ToByte();
        }

        public HmacSha256CryptoEncryptProvider(byte[] key) : base(Identity) {
            Hmac.Key = key;
        }
    }
    
    public class HmacSha512CryptoEncryptProvider : HmacCryptoEncryptProvider, IRuntimeMetadata<ICryptoProvider> {
        public const string    Identity = "HmacSha512";
        public override string Name     => Identity;
        public HmacSha512CryptoEncryptProvider(IOptionsMonitor<HmacCryptoOptions> options) : base(Identity) {
            if (options.CurrentValue != null && options.CurrentValue.CryptoName == Name) {
                Hmac.Key = options.CurrentValue.HmacSecretKey.ToByte();
            }
        }

        public HmacSha512CryptoEncryptProvider(string key) : base(Identity) {
            Hmac.Key = key.ToByte();
        }

        public HmacSha512CryptoEncryptProvider(byte[] key) : base(Identity) {
            Hmac.Key = key;
        }
    }
}
