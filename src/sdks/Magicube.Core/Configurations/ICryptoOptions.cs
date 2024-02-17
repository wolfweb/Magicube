using Magicube.Core.Encrypts;
using RSAExtensions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Magicube.Core {
    public interface ICryptoOptions {
        string CryptoName                         { get; set; } 
    }

    public abstract class CanGenerateSecretCryptoOptions : ICryptoOptions {
        public abstract string CryptoName         { get; set; } 
        public virtual  bool   GenerateSecretKey  { get; set; } = true;
    }

    public class HashCryptoOption : ICryptoOptions {
        public string CryptoName                  { get; set; } = "Sha512";
    }

    public class HmacCryptoOptions : ICryptoOptions {
        public string CryptoName                  { get; set; } = "HmacSha512";
        public string HmacSecretKey               { get; set; }
    }

    public class SymmetricCryptoOptions : CanGenerateSecretCryptoOptions {
        public override string CryptoName         { get; set; } = "TripleDes";
        public override bool   GenerateSecretKey  { get; set; }
        public string          SymmetricCryptoIv  { get; set; }
        public string          SymmetricCryptoKey { get; set; }
        public CipherMode      CipherMode         { get; set; } = CipherMode.CBC;
        public PaddingMode     PaddingMode        { get; set; } = PaddingMode.PKCS7;
    }

    public class RasCryptoOptions: CanGenerateSecretCryptoOptions {
        public override string        CryptoName         { get; set; } = "Ras";
        public override bool          GenerateSecretKey  { get; set; }
        public          string        PublicKey          { get; set; }
        public          string        PrivateKey         { get; set; }
        public          RSAKeyType    Type               { get; set; } = RSAKeyType.Pkcs8;
    }

    public class CryptoConfiguration {
        private readonly Dictionary<string, Type> _cryptoServices = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        public bool TryGetCrypto(string key, out Type cryptoService) { 
            cryptoService = null;
            if (_cryptoServices.ContainsKey(key)) {
                cryptoService = _cryptoServices[key];
            }

            return true;
        }

        public CryptoConfiguration RigisterCrypto<T>(string key) where T : ICryptoProvider {
            _cryptoServices.TryAdd(key, typeof(T));
            return this;
        }
    }
}
