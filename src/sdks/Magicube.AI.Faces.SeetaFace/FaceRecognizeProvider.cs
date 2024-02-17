using Magicube.Core;
using SixLabors.ImageSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ViewFaceCore;
using ViewFaceCore.Configs;
using ViewFaceCore.Core;

namespace Magicube.AI.Faces {
    public class FaceRecognizeProvider : IFaceRecognizeProvider {
		private readonly FaceDetector _faceDetector;
		private readonly FaceRecognizer _faceRecognizer;
		private readonly FaceLandmarker _faceLandmarker;

		public FaceRecognizeProvider() {
			_faceDetector   = new FaceDetector();
			_faceRecognizer = new FaceRecognizer();
			_faceLandmarker = new FaceLandmarker(new FaceLandmarkConfig { MarkType = ViewFaceCore.Model.MarkType.Normal });
		}

		public void Dispose() {
			_faceDetector?.Dispose();
			_faceRecognizer?.Dispose();
			_faceLandmarker?.Dispose();
		}

		public double FaceCompare(string source, string target) {
			var base64 = new ArrayBase64<float>();
			var sourceFeature = base64.FromBase64String(source).ToArray();
			var targetFeature = base64.FromBase64String(target).ToArray();

			return _faceRecognizer.Compare(sourceFeature, targetFeature);
		}

		public IEnumerable<string> GetFaceFeature(string path) {
			var image = Image.Load(path);
			return GetFaceFeature(image);
		}

		public IEnumerable<string> GetFaceFeature(byte[] datas) {
			var image = Image.Load(datas);
			return GetFaceFeature(image);
		}

        public void GetFaceLandmarks(string file) {
            var image = Image.Load(file);
            var faceInfos = _faceDetector.Detect(image);
			foreach (var face in faceInfos) {
				_faceLandmarker.Mark(image, face);
			}
        }

        IEnumerable<string> GetFaceFeature(Image image) {
			var faceInfos = _faceDetector.Detect(image);
			foreach (var face in faceInfos) {
				var facePoints = _faceLandmarker.Mark(image, face);
				var features = _faceRecognizer.Extract(image, facePoints);
				yield return new ArrayBase64<float>().ToBase64String(features);
			}
		}
	}
}