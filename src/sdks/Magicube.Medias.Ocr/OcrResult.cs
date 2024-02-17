using OpenCvSharp;
using System.Collections.Generic;

namespace Magicube.Medias.Ocr {
	public record OcrResult {
        public string                       Text    { get; set; }
        public IEnumerable<OcrResultRegion> Regions { get; set; }
	}

	public record OcrResultRegion {
		public RotatedRect Rect  { get; set; }
		public string      Text  { get; set; }
		public float       Score { get; set; }
	}
}