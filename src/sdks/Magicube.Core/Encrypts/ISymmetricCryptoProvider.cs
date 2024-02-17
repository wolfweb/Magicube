using Magicube.Core.Runtime;
using Magicube.Core.Runtime.Attributes;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace Magicube.Core.Encrypts {
    public interface ISymmetricCryptoProvider : ICryptoDecryptProvider {
        string IV                { get; set; }
        string Key               { get; set; }
        bool   GenerateSecretKey { get; set; }
    }

    public abstract class SymmetricCryptoProvider : ISymmetricCryptoProvider {
        private   int _initialized = 0;
        private static ConcurrentDictionary<string, SymmetricAlgorithm> _symmetricCrypto = new ConcurrentDictionary<string, SymmetricAlgorithm>();

        private readonly string _name;
        protected readonly SymmetricAlgorithm Algorithm;
        public SymmetricCryptoProvider(string name, SymmetricCryptoOptions options) {
            _name      = name;
            Algorithm  = _symmetricCrypto.GetOrAdd(name, x => SymmetricAlgorithm.Create(x));
            if (options != null) {
                GenerateSecretKey = options.GenerateSecretKey;

                Algorithm.Mode    = options.CipherMode;
                Algorithm.Padding = options.PaddingMode;

                if (options.GenerateSecretKey && options.SymmetricCryptoIv.IsNullOrEmpty() && options.SymmetricCryptoKey.IsNullOrEmpty()) {
                    EnsureIntilizated();
                }
                else if (!options.SymmetricCryptoIv.IsNullOrEmpty() && !options.SymmetricCryptoKey.IsNullOrEmpty()) {
                    IV  = options.SymmetricCryptoIv;
                    Key = options.SymmetricCryptoKey;
                } else {
                    throw new MagicubeException(10000, "symmetric crypto should auto generate secret key or given iv and key");
                }
            }
        }
        public          string   IV                { get; set; }
        public          string   Key               { get; set; }
        public          bool     GenerateSecretKey { get; set; }
        public abstract string   Name              { get; }
        public          string   Group             => "对称加密";

        [RuntimeMethod]
        public string Decrypt(string data, string charSet = "utf-8") {
            if (Key.IsNullOrEmpty() || IV.IsNullOrEmpty()) throw new InvalidDataException("invalid key and iv for this decrypt provider!");
            byte[] output = Transform(Convert.FromBase64String(data), Algorithm.CreateDecryptor(Key.FromBase64(), IV.FromBase64()));
            return output.ToString(charSet);
        }

        public byte[] Decrypt(byte[] data) {
            if (Key.IsNullOrEmpty() || IV.IsNullOrEmpty()) throw new InvalidDataException("invalid key and iv for this decrypt provider!");
            return Transform(data, Algorithm.CreateDecryptor(Key.FromBase64(), IV.FromBase64()));
        }

        public byte[] Decrypt(Stream data) {
            using(var mem = new MemoryStream()) {
                data.CopyTo(mem);
                return Decrypt(mem.ToArray());
            }
        }

        [RuntimeMethod]
        public string Encrypt(string data, string charSet = "utf-8") {
            if (GenerateSecretKey) EnsureIntilizated();

            byte[] output = Transform(data.ToByte(charSet), Algorithm.CreateEncryptor(Key.FromBase64(), IV.FromBase64()));
            return Convert.ToBase64String(output);
        }

        public byte[] Encrypt(byte[] data) {
            if (GenerateSecretKey) EnsureIntilizated();
            return Transform(data, Algorithm.CreateEncryptor(Key.FromBase64(), IV.FromBase64()));
        }

        public byte[] Encrypt(Stream data) {
            using (var stream = new MemoryStream()) {
                data.CopyTo(stream);
                return Encrypt(stream.ToArray());
            }
        }

        private byte[] Transform(byte[] input, ICryptoTransform cryptoTransform) {
            using (MemoryStream memStream = new MemoryStream()) {
                using (CryptoStream cryptStream = new CryptoStream(memStream, cryptoTransform, CryptoStreamMode.Write)) {
                    cryptStream.Write(input, 0, input.Length);
                    cryptStream.FlushFinalBlock();
                    memStream.Position = 0;
                    byte[] result = memStream.ToArray();
                    return result;
                }
            }
        }

        private void EnsureIntilizated() {
            if(Interlocked.CompareExchange(ref _initialized, 1, 0) == 0) {
                Algorithm.GenerateIV();
                Algorithm.GenerateKey();

                IV  = Algorithm.IV.ToBase64();
                Key = Algorithm.Key.ToBase64();
            }
        }

        public void Dispose() {
            Algorithm.Dispose();
            _symmetricCrypto.TryRemove(_name, out _);
        }
    }

    public class AesSymmetricCryptoProvider : SymmetricCryptoProvider, IRuntimeMetadata<ICryptoProvider> {
        public const    string Identity = "Aes";
        public override string Name     => Identity;

        public AesSymmetricCryptoProvider(IOptionsMonitor<SymmetricCryptoOptions> options) : this(options.CurrentValue.CryptoName == Identity ? options.CurrentValue : null) { }

        public AesSymmetricCryptoProvider(SymmetricCryptoOptions options) : base(Identity, options) { }
    }

    public class DesSymmetricCryptoProvider : SymmetricCryptoProvider, IRuntimeMetadata<ICryptoProvider> {
        public const string Identity = "Des";
        public override string Name => Identity;
        public DesSymmetricCryptoProvider(IOptionsMonitor<SymmetricCryptoOptions> options) : this(options.CurrentValue.CryptoName == Identity ? options.CurrentValue : null) { }

        public DesSymmetricCryptoProvider(SymmetricCryptoOptions options) : base(Identity, options) { }
    }

    public class Rc2SymmetricCryptoProvider : SymmetricCryptoProvider, IRuntimeMetadata<ICryptoProvider> {
        public const string Identity = "Rc2";
        public override string Name => Identity;
        public Rc2SymmetricCryptoProvider(IOptionsMonitor<SymmetricCryptoOptions> options) : this(options.CurrentValue.CryptoName == Identity ? options.CurrentValue : null) { }

        public Rc2SymmetricCryptoProvider(SymmetricCryptoOptions options) : base (Identity, options) { }
    }

    public class TripleDesSymmetricCryptoProvider : SymmetricCryptoProvider, IRuntimeMetadata<ICryptoProvider> {
        public const string Identity = "TripleDes";
        public override string Name => Identity;
        public TripleDesSymmetricCryptoProvider(IOptionsMonitor<SymmetricCryptoOptions> options) : this(options.CurrentValue.CryptoName == Identity ? options.CurrentValue : null) { }

        public TripleDesSymmetricCryptoProvider(SymmetricCryptoOptions options) : base(Identity, options) { }
    }
}
