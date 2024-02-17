using Microsoft.Extensions.Logging;
using NsfwSpyNS;
using System.IO;

namespace Magicube.Medias.Nsfw {
    public interface IMediaNsfwService {
        NsfwSpyResult ClassifyImage(Stream stream);
        NsfwSpyResult ClassifyImage(byte[] bytes);
    }

    public class MediaNsfwService : IMediaNsfwService {
        private readonly ILogger _logger;
        private readonly INsfwSpy _nsfwSpy;

        public MediaNsfwService(ILogger<IMediaNsfwService> logger) {
            _logger = logger;
            _nsfwSpy = new NsfwSpy();
        }

        public NsfwSpyResult ClassifyImage(Stream stream) {
            using (var ms = new MemoryStream()) {
                stream.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return ClassifyImage(ms.ToArray());
            }
        }

        public NsfwSpyResult ClassifyImage(byte[] bytes) {
            var result = _nsfwSpy.ClassifyImage(bytes);
            return result;
        }
    }
}