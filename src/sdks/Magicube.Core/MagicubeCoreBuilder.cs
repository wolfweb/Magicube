using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Magicube.Core.Runtime;
using Magicube.Core.Encrypts;

namespace Magicube.Core {
    public class MagicubeCoreBuilder {
        private readonly IServiceCollection _services;
        public MagicubeCoreBuilder(IServiceCollection services) {
            _services            = services;
            AddDefaultStaticRuntimeMetadata();
        }

        public MagicubeCoreBuilder ConfigHashCryptoOptions(Action<HashCryptoOption> builder = null) {
            _services.Configure<HashCryptoOption>(options=> {
                builder?.Invoke(options);
            });

            _services.Configure<CryptoConfiguration>(options => {
                options.RigisterCrypto<Md5CryptoEncryptProvider>(Md5CryptoEncryptProvider.Identity);
                options.RigisterCrypto<Sha1CryptoEncryptProvider>(Sha1CryptoEncryptProvider.Identity);
                options.RigisterCrypto<Sha256CryptoEncryptProvider>(Sha256CryptoEncryptProvider.Identity);
                options.RigisterCrypto<Sha384CryptoEncryptProvider>(Sha384CryptoEncryptProvider.Identity);
                options.RigisterCrypto<Sha512CryptoEncryptProvider>(Sha512CryptoEncryptProvider.Identity);
            });

            return this;
        }

        public MagicubeCoreBuilder ConfigHmacCryptoOptions(Action<HmacCryptoOptions> builder) {
            _services.Configure<HmacCryptoOptions>(options=> {
                builder.Invoke(options);
                if (options.HmacSecretKey.IsNullOrEmpty()) throw new MagicubeException(10000, "hmac crypto require secret key");            
            });

            _services.Configure<CryptoConfiguration>(options => {
                options.RigisterCrypto<HmacMd5CryptoEncryptProvider>(HmacMd5CryptoEncryptProvider.Identity);
                options.RigisterCrypto<HmacSha1CryptoEncryptProvider>(HmacSha1CryptoEncryptProvider.Identity);
                options.RigisterCrypto<HmacSha256CryptoEncryptProvider>(HmacSha256CryptoEncryptProvider.Identity);
                options.RigisterCrypto<HmacSha512CryptoEncryptProvider>(HmacSha512CryptoEncryptProvider.Identity);
            });

            return this;
        }

        public MagicubeCoreBuilder ConfigSymmetricCryptoOptions(Action<SymmetricCryptoOptions> builder) {
            _services.Configure<SymmetricCryptoOptions>(options => {
                builder.Invoke(options);
                if (!options.GenerateSecretKey && (options.SymmetricCryptoIv.IsNullOrEmpty() || options.SymmetricCryptoKey.IsNullOrEmpty())) throw new MagicubeException(10000, "symmetric crypto require iv and key");
            });

            _services.Configure<CryptoConfiguration>(options => {
                options.RigisterCrypto<AesSymmetricCryptoProvider>(AesSymmetricCryptoProvider.Identity);
                options.RigisterCrypto<DesSymmetricCryptoProvider>(DesSymmetricCryptoProvider.Identity);
                options.RigisterCrypto<Rc2SymmetricCryptoProvider>(Rc2SymmetricCryptoProvider.Identity);
                options.RigisterCrypto<TripleDesSymmetricCryptoProvider>(TripleDesSymmetricCryptoProvider.Identity);
            });

            return this;
        }

        public MagicubeCoreBuilder ConfigRasCryptoOptions(Action<RasCryptoOptions> builder) {
            _services.Configure<RasCryptoOptions>(options=> {
                builder.Invoke(options);
                if (!options.GenerateSecretKey && (options.PublicKey.IsNullOrEmpty() || options.PrivateKey.IsNullOrEmpty())) throw new MagicubeException(10000, "ras crypto require public key and private key");
            });

            _services.Configure<CryptoConfiguration>(options => {
                options.RigisterCrypto<RasCryptoEncryptProvider>("ras");
            });

            return this;
        }

        public MagicubeCoreBuilder AddStaticRuntimeMetadata(Type type){
            if(type.IsAbstract && type.IsSealed && !type.IsGenericType && !type.IsNested) {
                _services.Configure<StaticRuntimeMetadataOptions>(x => x.RegisterRuntimeMetadata(type));
            }
            return this;
        }

        internal void Build() {
            var hashOptions = _services.LastOrDefault(x=>x.ServiceType == typeof(IConfigureOptions<HashCryptoOption>));
            if (hashOptions == null) {
                ConfigHashCryptoOptions();
            }

            var hmacOptions = _services.LastOrDefault(x => x.ServiceType == typeof(IConfigureOptions<HmacCryptoOptions>));
            if (hmacOptions == null) {
                ConfigHmacCryptoOptions(x => x.HmacSecretKey = Guid.NewGuid().ToString("n"));
            }

            var symmetricOptions = _services.LastOrDefault(x => x.ServiceType == typeof(IConfigureOptions<SymmetricCryptoOptions>));
            if (symmetricOptions == null) {
                ConfigSymmetricCryptoOptions(x => x.GenerateSecretKey = true);
            }

            var rasOptions = _services.LastOrDefault(x => x.ServiceType == typeof(IConfigureOptions<RasCryptoOptions>));
            if (rasOptions == null) {
                ConfigRasCryptoOptions(x => x.GenerateSecretKey = true);
            }
        }

        private void AddDefaultStaticRuntimeMetadata() {
            AddStaticRuntimeMetadata(typeof(DateTimeExtension))
            .AddStaticRuntimeMetadata(typeof(StringExtension));
        }
    }
}