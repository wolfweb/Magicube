using Magicube.Data.Abstractions;
using Magicube.Web.UI.Entities;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using Magicube.Core;
using Microsoft.Extensions.Primitives;
using Magicube.Core.Signals;

namespace Magicube.Web.UI.Liquid.FileProviders {
    public class LiquidViewInfo : IFileInfo {
        private readonly IRepository<WebPage, int> _rep;

        private bool           _exists;
        private string         _viewPath;
        private byte[]         _viewContent;
        private DateTimeOffset _lastModified;

        public LiquidViewInfo(string viewPath, ISignal signal, IRepository<WebPage, int> rep) {
            _rep        = rep;
            _viewPath   = viewPath.TrimEnd(".liquid").ToLower();
            (var _, ChangeToken) = signal.GetToken(viewPath);
            GetView();
        }
        public bool           Exists       => _exists;

        public bool           IsDirectory  => false;

        public DateTimeOffset LastModified => _lastModified;

        public string         Name         => Path.GetFileName(_viewPath);

        public string         PhysicalPath => null;

        public IChangeToken   ChangeToken  { get; }

        public long Length {
            get {
                using (var stream = new MemoryStream(_viewContent)) {
                    return stream.Length;
                }
            }
        }

        public Stream CreateReadStream() {
            return new MemoryStream(_viewContent);
        }

        private void GetView() {
            var entity = _rep.Get(x => x.Path == _viewPath);
            if (entity != null) {
                _exists       = true;
                _viewContent  = entity.Body.ToByte();
                _lastModified = DateTimeOffset.FromUnixTimeSeconds(entity.UpdateAt.GetValueOrDefault(DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
            }
        }
    }
}
