using Magicube.Core;
using Magicube.Core.Encrypts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace Magicube.Core.Test {
    public class CryptoProviderTest {
        private const string rawStr = "123456";
        private ICryptoEncryptProvider CryptoEncryptProvider;

        [Fact]
        public void Func_HashCrypto_Test() {
            CryptoEncryptProvider = new Md5CryptoEncryptProvider();
            var actual = CryptoEncryptProvider.Encrypt(rawStr);
            Assert.Equal("e10adc3949ba59abbe56e057f20f883e", actual.ToLower());

            CryptoEncryptProvider = new Sha1CryptoEncryptProvider();
            actual = CryptoEncryptProvider.Encrypt(rawStr);
            Assert.Equal("7c4a8d09ca3762af61e59520943dc26494f8941b", actual.ToLower());

            CryptoEncryptProvider = new Sha256CryptoEncryptProvider();
            actual = CryptoEncryptProvider.Encrypt(rawStr);
            Assert.Equal("8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92", actual.ToLower());

            CryptoEncryptProvider = new Sha384CryptoEncryptProvider();
            actual = CryptoEncryptProvider.Encrypt(rawStr);
            Assert.Equal("0a989ebc4a77b56a6e2bb7b19d995d185ce44090c13e2984b7ecc6d446d4b61ea9991b76a4c2f04b1b4d244841449454", actual.ToLower());

            CryptoEncryptProvider = new Sha512CryptoEncryptProvider();
            actual = CryptoEncryptProvider.Encrypt(rawStr);
            Assert.Equal("ba3253876aed6bc22d4a6ff53d8406c6ad864195ed144ab5c87621b6c233b548baeae6956df346ec8c17f5ea10f35ee3cbc514797ed7ddd3145464e2a0bab413", actual.ToLower());
        }

        [Fact]
        public void Func_HmacCrypto_Test() {
            var options = new Mock<IOptionsMonitor<HmacCryptoOptions>>();
            options.Setup(x => x.CurrentValue).Returns(new HmacCryptoOptions { HmacSecretKey = "wolfweb" });

            options.Object.CurrentValue.CryptoName = "HmacMd5";
            CryptoEncryptProvider = new HmacMd5CryptoEncryptProvider(options.Object);
            var acutal = CryptoEncryptProvider.Encrypt(rawStr);
            Assert.Equal("779161a5396c1cd76700eae47d379491", acutal.ToLower());

            options.Object.CurrentValue.CryptoName = "HmacSha1";
            CryptoEncryptProvider = new HmacSha1CryptoEncryptProvider(options.Object);
            acutal = CryptoEncryptProvider.Encrypt(rawStr);
            Assert.Equal("2b1f030b3152b4c585e88171027fb305c262aa7e", acutal.ToLower());

            options.Object.CurrentValue.CryptoName = "HmacSha256";
            CryptoEncryptProvider = new HmacSha256CryptoEncryptProvider(options.Object);
            acutal = CryptoEncryptProvider.Encrypt(rawStr);
            Assert.Equal("5b0f2e06b5dce38d7930afbd73b5962591688b95dc23915bb8e73826c11f48d9", acutal.ToLower());

            options.Object.CurrentValue.CryptoName = "HmacSha512";
            CryptoEncryptProvider = new HmacSha512CryptoEncryptProvider(options.Object);
            acutal = CryptoEncryptProvider.Encrypt(rawStr);
            Assert.Equal("4a0329b43c43ef7f6e48e8630d488e6a782b554a9da685f6dd2ed7058f64486a7a52e31da96ca3eb6d6a8a71c64be96001d08d1f3e259b8568d434931d99f0b0", acutal.ToLower());
        }

        [Fact]
        public void Func_SymmetricCrypto_Test() {
            var options = new Mock<IOptionsMonitor<SymmetricCryptoOptions>>();
            options.Setup(x => x.CurrentValue).Returns(new SymmetricCryptoOptions() {
                GenerateSecretKey = true,
                CryptoName = "Aes"
            });

            SymmetricCryptoProvider CryptoProvider = new AesSymmetricCryptoProvider(options.Object);
            var encryptStr = CryptoProvider.Encrypt(rawStr);
            var decryptStr = CryptoProvider.Decrypt(encryptStr);
            Assert.Equal(rawStr, decryptStr);

            options.Object.CurrentValue.CryptoName = "Des";
            CryptoProvider = new DesSymmetricCryptoProvider(options.Object) {
                GenerateSecretKey = true
            };
            encryptStr = CryptoProvider.Encrypt(rawStr);
            decryptStr = CryptoProvider.Decrypt(encryptStr);
            Assert.Equal(rawStr, decryptStr);

            options.Object.CurrentValue.CryptoName = "Rc2";
            CryptoProvider = new Rc2SymmetricCryptoProvider(options.Object) {
                GenerateSecretKey = true
            };
            encryptStr = CryptoProvider.Encrypt(rawStr);
            decryptStr = CryptoProvider.Decrypt(encryptStr);
            Assert.Equal(rawStr, decryptStr);

            options.Object.CurrentValue.CryptoName = "TripleDes";
            CryptoProvider = new TripleDesSymmetricCryptoProvider(options.Object) {
                GenerateSecretKey = true
            };
            encryptStr = CryptoProvider.Encrypt(rawStr);
            decryptStr = CryptoProvider.Decrypt(encryptStr);
            Assert.Equal(rawStr, decryptStr);
        }

        [Fact]
        public void Func_RasCrypt_Test() {
            var privateKey = @"MIIEowIBAAKCAQEA5eA23vq9QJSkKOtoObCNd8hpljJSYd03z+WGBc1gyGzSbWzIHDm+bIlHacB7sfxkg27qLv/JTuLPJyLC517CFoC5oBD3LKjm7hy9suhRb1WR2j8lLFxiqqeKAvHBHKV7bJtRQrPSiVXvXFJOhNr/XiKtfn4DolLV+1OIXk5cWfWbr2TXBtGJFW0BXRCskH1nyTikU/6F6HBN7lHz47ZKnRkZ3q15lyruDfte+vxlVWDXEG1CgFJBbqrGyIWCW18IyyI87todk/iInyEDeG+/NsKGZoYsqWBFhbpeMu6r0L6k6yOx52rkZSiQ5l33VsEnOAFhm+RL9jeqm6+F6qx4HQIDAQABAoIBADN0wxOasO5Z131JZKU1PhWICQqT7Rj2+d1RlXSLCpUStu/Dn++hhVyqRhAIBChNRPqew1EN9LPx1Uj/YP2FmpCK9AI+ifW3QSofyN7ZXhE76FAgPmP1sihdJGmQUBfnev9OrRvvQLt5PwE9c1IN64fY31knQAz/2eJOSoJBrC5+fWILbdmg8nTioF+chkX/9YkF2Ji55i4WCx5t/AK1dwQIBLUIUVTA6MjJ+ODTAhydYjZRrKkxQd8qoyslNO1eNX8XigUFEv/FacsEXy7Iqih45ug+F+24pBXdkmlO6QvfLOZnWUDibDWXXhfESCQ4k/pgH/TA1XOcA/5e7nOhh0kCgYEA9t+FOmCVk6HVub3npq+DOwT5DGL3npQWdRedkUJfQlEzMup5TxpmOSmfbmcxgIQTatE4/T5myO+075oULyoBpYBnOiu4Gdb8B7if8RhZAjmHjxefWgEexmfdV2FEJhcjEKT3hmgxBUQUJkT9DyRt0vfwP0dMWgoHzhAiV1bc9kMCgYEA7l/Tm5V3vjS3DYHT3bu70IpkoVrWsHcS3zekCznDMibZkbT62hdgVsTUh3Dto5gJ1vrVtMlWdYWeF9toAU3/srRJ14KL8HVB3Bbxsb+vjdFgQFYb1bJm8ujsn2Q6WAaYh4H0McZuGXvUhJypDJ/nD+6bF7SF5cTutT1bVCf3Yh8CgYEA6AUnV7hSnA/rqMgcoYIvGhcQYl3ZT8bqXF43jsAMe22Jav8HyBqNnIfL+Z0u3xRCk4/Tud8eBxeSu+XZtoIKThuh6QAR5Ocys6cHWzaA4SBkkU2oTJTk8Z/IXUljHVF9eTUyFbZy6/oR3e3U0JhyR2cS+sXtsmne8AIl5GQVA7UCgYBWxDyzIinO60NCQGKNEFuh4e4VKYqB+yW8aHVmvKTHaYrCVb0Fi4K+srCliD6H0LysKuuE/dBhwLw19OAbsXeEZcmHD3a+lP/fC974E0zkczT01iMVmvWML6qJriLqjaQRlwT65T6IOiG2D4wdE0s46mI7s7MVWqSFBgtrS4zpgwKBgB+6wy6enxRSuAtnzLhnops5XCAl1tH/jJgfWW0LmPLEM2cioGgbah4fnJ9mS/7QBT9IH0gEfSsNFG4D/4WlNvCmWuuM9HyS++HulZW8OFjmtnhJsitYLwEC7JwkgTr4oi11VnCjtF0JTLlOHtBf7E+XqRrZyFQEQVMdt2AjuNAT";

            var publicKey = @"MIIBCgKCAQEA5eA23vq9QJSkKOtoObCNd8hpljJSYd03z+WGBc1gyGzSbWzIHDm+bIlHacB7sfxkg27qLv/JTuLPJyLC517CFoC5oBD3LKjm7hy9suhRb1WR2j8lLFxiqqeKAvHBHKV7bJtRQrPSiVXvXFJOhNr/XiKtfn4DolLV+1OIXk5cWfWbr2TXBtGJFW0BXRCskH1nyTikU/6F6HBN7lHz47ZKnRkZ3q15lyruDfte+vxlVWDXEG1CgFJBbqrGyIWCW18IyyI87todk/iInyEDeG+/NsKGZoYsqWBFhbpeMu6r0L6k6yOx52rkZSiQ5l33VsEnOAFhm+RL9jeqm6+F6qx4HQIDAQAB";
            var options = new Mock<IOptionsMonitor<RasCryptoOptions>>();
            options.Setup(x => x.CurrentValue).Returns(new RasCryptoOptions {
                PublicKey  = publicKey,
                PrivateKey = privateKey,
                Type       = RSAExtensions.RSAKeyType.Pkcs1
            });

            IAsymmetricCryptoDecryptProvider provider = new RasCryptoEncryptProvider(options.Object);
            var encryptStr = provider.Encrypt(rawStr);
            var result = provider.Decrypt(encryptStr);
            Assert.Equal(rawStr, result);

            provider.GenerateSecretKey = true;
            encryptStr = provider.Encrypt(rawStr);
            result = provider.Decrypt(encryptStr);
            Assert.Equal(rawStr, result);
        }

        [Fact]
        public void Func_AsymmetricCrypto_Test() {
            var container = new ServiceCollection()
                .BuildServiceProvider();

            var options = new Mock<IOptionsMonitor<RasCryptoOptions>>();
            options.Setup(x => x.CurrentValue).Returns(new RasCryptoOptions {
                GenerateSecretKey = true
            });

            IAsymmetricCryptoDecryptProvider provider = new RasCryptoEncryptProvider(options.Object);

            //var createClientServerAuthCerts = container.GetService<CreateCertificatesClientServerAuth>();
            //var cert = createClientServerAuthCerts.NewClientSelfSignedCertificate(
            //    new DistinguishedName { Country = "CN", Organisation = "wolfweb的工作室", CommonName = "wolfweb的工作室" },
            //    new ValidityPeriod { ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(1) },
            //    "wolfweb的工作室"
            //    );

        }

        [Fact]
        public void Func_CryptoProvider_DI_Test() {
            var providers = new ServiceCollection()
                .AddCore(builder => {

                })
                .BuildServiceProvider();

            var factory = providers.GetService<CryptoServiceFactory>();
            var md5Crypto = factory.GetCryptoService<Md5CryptoEncryptProvider>(Md5CryptoEncryptProvider.Identity);
            Assert.NotNull(md5Crypto);

            var md5Crypto1 = factory.GetCryptoService<ICryptoEncryptProvider>(Md5CryptoEncryptProvider.Identity);
            Assert.NotEqual(md5Crypto1, md5Crypto);


            var hmacSha1_1 = factory.GetCryptoService<ICryptoEncryptProvider>(HmacSha1CryptoEncryptProvider.Identity, "hello");
            var hmacSha1_2 = factory.GetCryptoService<ICryptoEncryptProvider>(HmacSha1CryptoEncryptProvider.Identity, "hello2");
            Assert.NotEqual(hmacSha1_1, hmacSha1_2);
        }
    }
}
