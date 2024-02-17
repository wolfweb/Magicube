using System;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using Magicube.Core.Encrypts;
using Magicube.Core;

namespace Magicube.Storage.Upyun.Internal {
    interface IUpyunSecurity {
        void Signature(HttpWebRequest request);
        void Signature(HttpWebRequest request, long length);
        Task Signature(HttpRequestMessage request);
    }

    abstract class Security : IUpyunSecurity {
        private readonly NetworkCredential _credential;
        protected readonly CryptoServiceFactory _cryptoFactory;

        protected NetworkCredential Credential {
            get { return _credential; }
        }

        public Security(string username, string password, CryptoServiceFactory factory) : this(new NetworkCredential(username, password), factory) {
        }

        public Security(NetworkCredential credential, CryptoServiceFactory factory) {
            _credential = credential;
            _cryptoFactory = factory;
        }

        public abstract void Signature(HttpWebRequest request);
        public abstract void Signature(HttpWebRequest request, long length);
        public abstract Task Signature(HttpRequestMessage request);

        public string GetPasswordHash() {
            return _cryptoFactory.GetCryptoService<Md5CryptoEncryptProvider>(Md5CryptoEncryptProvider.Identity).Encrypt(Credential.Password);
        }

        public async Task<long> GetRequestLength(HttpRequestMessage request) {
            if (!request.Method.Equals(HttpMethod.Get) && request.Content != null) {
                var stream = await request.Content.ReadAsStreamAsync();
                return stream.Length;
            }
            return 0L;
        }
    }

    class BasicSecurity : Security {
        public BasicSecurity(NetworkCredential credential, CryptoServiceFactory factory)
            : base(credential, factory) {
        }

        public BasicSecurity(string username, string password, CryptoServiceFactory factory)
            : base(username, password, factory) {
        }

        public override void Signature(HttpWebRequest request) {
            var buffer = Encoding.ASCII.GetBytes(string.Concat(Credential.UserName, ":", Credential.Password));
            var authorization = Convert.ToBase64String(buffer);
            request.Headers.Add("Authorization", string.Format("Basic {0}", authorization));
        }

        public override void Signature(HttpWebRequest request, long length) {
            Signature(request);
        }

        public override Task Signature(HttpRequestMessage request) {
            var buffer = Encoding.ASCII.GetBytes(string.Concat(Credential.UserName, ":", Credential.Password));
            var authorization = Convert.ToBase64String(buffer);
            request.Headers.Add("Authorization", string.Format("Basic {0}", authorization));
            return Task.FromResult<object>(null);
        }
    }

    class Md5Security : Security {
        public Md5Security(string username, string password, CryptoServiceFactory factory)
            : base(username, password, factory) {
        }

        public Md5Security(NetworkCredential credential, CryptoServiceFactory factory)
            : base(credential, factory) {
        }

        public override void Signature(HttpWebRequest request) {
            Signature(request, request.ContentLength);
        }

        public override void Signature(HttpWebRequest request, long length) {
            request.Date = DateTime.UtcNow;
            var required = string.Format("{0}&{1}&{2}&{3}&{4}",
                request.Method,
                request.RequestUri.AbsolutePath,
                request.Headers.Get("Date"),
                length,
                GetPasswordHash());

            var signature = _cryptoFactory.GetCryptoService<Md5CryptoEncryptProvider>(Md5CryptoEncryptProvider.Identity).Encrypt(required);

            request.Headers.Add("Authorization", string.Format("Upyun {0}:{1}", Credential.UserName, signature));
        }

        public override async Task Signature(HttpRequestMessage request) {
            request.Headers.Date = DateTime.UtcNow;
            var length = await GetRequestLength(request);
            var required = string.Format("{0}&{1}&{2}&{3}&{4}",
                request.Method,
                request.RequestUri.AbsolutePath,
                request.Headers.Date.Value.ToString("r"),
                length,
                GetPasswordHash());

            var signature = _cryptoFactory.GetCryptoService<Md5CryptoEncryptProvider>(Md5CryptoEncryptProvider.Identity).Encrypt(required);

            request.Headers.Add("Authorization", string.Format("Upyun {0}:{1}", Credential.UserName, signature));
        }
    }

    class HamcSha1Security : Security {
        public HamcSha1Security(NetworkCredential credential, CryptoServiceFactory factory)
            : base(credential, factory) {
        }

        public HamcSha1Security(string username, string password, CryptoServiceFactory factory)
            : base(username, password, factory) {
        }

        public override void Signature(HttpWebRequest request) {
            var key = GetPasswordHash();
            request.Date = DateTime.UtcNow;
            var required = string.Format("{0}&{1}&{2}",
                request.Method,
                request.RequestUri.AbsolutePath,
                request.Headers.Get("Date"));

            var signature = _cryptoFactory.GetCryptoService<HmacSha1CryptoEncryptProvider>(HmacSha1CryptoEncryptProvider.Identity, key).Encrypt(required.ToByte()).ToBase64(); ;

            request.Headers.Add("Authorization", string.Format("Upyun {0}:{1}", Credential.UserName, signature));
        }

        public override void Signature(HttpWebRequest request, long length) {
            Signature(request);
        }

        public override Task Signature(HttpRequestMessage request) {
            var key = GetPasswordHash();
            request.Headers.Date = DateTime.UtcNow;
            var required = string.Format("{0}&{1}&{2}",
                request.Method,
                request.RequestUri.AbsolutePath,
                request.Headers.Date.Value.ToString("r"));

            var signature = _cryptoFactory.GetCryptoService<HmacSha1CryptoEncryptProvider>(HmacSha1CryptoEncryptProvider.Identity, key).Encrypt(required.ToByte()).ToBase64();

            request.Headers.Add("Authorization",
                string.Format("Upyun {0}:{1}", Credential.UserName, signature));
            return Task.FromResult<object>(null);
        }
    }
}