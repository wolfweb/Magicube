using Magicube.Core.Runtime;
using Magicube.Core.Runtime.Attributes;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Magicube.Core.Encrypts {
    public abstract class HashCryptoEncryptProvider : ICryptoEncryptProvider {
        protected static ConcurrentDictionary<string, HashAlgorithm> _hashCryptos = new ConcurrentDictionary<string, HashAlgorithm>();

        public abstract string Name  { get; }
        public          string Group => "Hash加密";

        private readonly HashAlgorithm _hashAlgorithm;
        private readonly string _name;

        public HashCryptoEncryptProvider(string name) {
            _name          = name;
            _hashAlgorithm = _hashCryptos.GetOrAdd(name, x => HashAlgorithm.Create(x));
        }

        [RuntimeMethod]
        public string  Encrypt(string data, string charSet = "utf-8") {
            var bytes  = data.ToByte(charSet);
            var result = _hashAlgorithm.ComputeHash(bytes);
            return result.Aggregate("", (current, t) => current + t.ToString("X").PadLeft(2, '0'));
        }
                       
        public byte[]  Encrypt(byte[] data) {
            return _hashAlgorithm.ComputeHash(data);
        }

        public byte[] Encrypt(Stream data) {
            return _hashAlgorithm.ComputeHash(data);
        }

        public void    Dispose() {
            _hashAlgorithm.Dispose();
            _hashCryptos.TryRemove(_name, out _);
        }
    }

    public class Md5CryptoEncryptProvider : HashCryptoEncryptProvider, IRuntimeMetadata<ICryptoProvider> {
        public const    string Identity = "Md5";
        public override string Name     => Identity;

        public Md5CryptoEncryptProvider() : base(Identity) {}
    }

    public class Sha1CryptoEncryptProvider : HashCryptoEncryptProvider, IRuntimeMetadata<ICryptoProvider> {
        public const    string Identity = "Sha1";
        public override string Name     => Identity;

        public Sha1CryptoEncryptProvider() : base(Identity) {
        }
    }

    public class Sha256CryptoEncryptProvider : HashCryptoEncryptProvider, IRuntimeMetadata<ICryptoProvider> {
        public const    string Identity = "Sha256";
        public override string Name     => Identity;

        public Sha256CryptoEncryptProvider() : base(Identity) {
        }
    }

    public class Sha384CryptoEncryptProvider : HashCryptoEncryptProvider, IRuntimeMetadata<ICryptoProvider> {
        public const    string Identity = "Sha384";
        public override string Name     => Identity;

        public Sha384CryptoEncryptProvider() : base(Identity) {
        }
    }

    public class Sha512CryptoEncryptProvider : HashCryptoEncryptProvider, IRuntimeMetadata<ICryptoProvider> {
        public const    string Identity = "Sha512";
        public override string Name     => Identity;

        public Sha512CryptoEncryptProvider() : base(Identity) {
        }
    }
}
