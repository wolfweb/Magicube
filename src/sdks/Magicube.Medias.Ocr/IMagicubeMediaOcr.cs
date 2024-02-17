using Microsoft.Extensions.Logging;
using OpenCvSharp;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR.Models.Local;
using Sdcb.PaddleOCR;
using System;
using System.IO;
using System.Linq;

namespace Magicube.Medias.Ocr {
    public interface IMagicubeMediaOcr : IDisposable {
		OcrResult Detect(Stream stream);
		OcrResult Detect(byte[] bytes);
    }

    public class PaddleMediaOcr : IMagicubeMediaOcr {
        private readonly ILogger _logger;
        private readonly PaddleOcrAll _ocrAll;
        public PaddleMediaOcr(ILogger<IMagicubeMediaOcr> logger) {
            _logger = logger;
            _ocrAll = new PaddleOcrAll(LocalFullModels.ChineseV4, PaddleDevice.Mkldnn()) {
                AllowRotateDetection = true,
                Enable180Classification = false
            };
        }

        public OcrResult Detect(Stream stream) {
            using (var mem = new MemoryStream()) {
                stream.CopyTo(mem);
                mem.Seek(0, SeekOrigin.Begin);
                return Detect(mem.ToArray());
            }
        }

        public OcrResult Detect(byte[] bytes) {
            using (var img = Cv2.ImDecode(bytes, ImreadModes.Color)) {
                PaddleOcrResult result = _ocrAll.Run(img);
                return new OcrResult {
                    Text = result.Text,
                    Regions = result.Regions.Select(x => new OcrResultRegion { 
                        Rect  = x.Rect,
                        Text  = x.Text,
                        Score = x.Score,
                    })
                };
            }
        }


        public void Dispose() {
            try {
                _ocrAll.Dispose();
            } catch (Exception ex) {
                _logger.LogError("Error:{0}", ex);
            }
        }
    }
}