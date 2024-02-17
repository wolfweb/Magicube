using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;

namespace Magicube.Core.Encrypts {
    public class CryptoServiceFactory {
        private readonly IServiceProvider _serviceProvider;
        private readonly CryptoConfiguration _cryptoConfiguration;

        public CryptoServiceFactory(
            IOptions<CryptoConfiguration> options, 
            IServiceProvider serviceProvider
            ) {
            _cryptoConfiguration = options.Value;
            _serviceProvider     = serviceProvider;
        }

        public T GetCryptoService<T>(string name, params object[] args) where T : ICryptoProvider {
            if (_cryptoConfiguration.TryGetCrypto(name, out var crypto)) {
                if (typeof(T).IsAssignableFrom(crypto)) {
                    return _serviceProvider.Resolve<T>(crypto, args);
                }
            }

            throw new MagicubeException(1000, "无效的加密服务类型");
        }
    }
}
