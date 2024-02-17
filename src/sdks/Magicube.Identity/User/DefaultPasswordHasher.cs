using Magicube.Core.Encrypts;
using Magicube.Web.Sites;
using Microsoft.AspNetCore.Identity;
using System;

namespace Magicube.Identity {
    public class DefaultPasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class {
        private readonly DefaultSite _siteSettings;
        private readonly CryptoServiceFactory _cryptoServiceFactory;
        public DefaultPasswordHasher(
            ISiteManager siteManager,
            CryptoServiceFactory cryptoServiceFactory) {
            _siteSettings         = siteManager.GetSite();
            _cryptoServiceFactory = cryptoServiceFactory;
        }

        public string HashPassword(TUser user, string password) {
            if (password == null) {
                throw new ArgumentNullException("password");
            }

            var provider = _cryptoServiceFactory.GetCryptoService<ICryptoEncryptProvider>(_siteSettings.UserPasswordCryptoType);
            if (provider == null) throw new ArgumentOutOfRangeException("password encrypt type");

            return provider.Encrypt(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword) {
            if (hashedPassword == null) {
                throw new ArgumentNullException("hashedPassword");
            }
            if (providedPassword == null) {
                throw new ArgumentNullException("providedPassword");
            }

            var provider = _cryptoServiceFactory.GetCryptoService<ICryptoEncryptProvider>(_siteSettings.UserPasswordCryptoType);
            if (provider == null) throw new ArgumentOutOfRangeException("password encrypt type");

            string expected = provider.Encrypt(providedPassword);

            return hashedPassword.Equals(expected) ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }
    }
}
