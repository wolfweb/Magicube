using FaceRecognitionDotNet;
using Magicube.Core;
using Microsoft.Extensions.Options;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.AI.Faces {
    public class FaceRecognizeProvider : IFaceRecognizeProvider {
		private readonly FaceRecognition _faceRecognition;
		private readonly FaceRecognizeOption _options;
		private readonly Model _model;
		public FaceRecognizeProvider(IOptions<FaceRecognizeOption> options) {
			_options         = options.Value;
			_model           = _options.UseCnn ? Model.Cnn : Model.Hog;
			_faceRecognition = FaceRecognition.Create(_options.ModelFolder);
		}

		public void Dispose() {
			if(_faceRecognition != null ) {
				_faceRecognition.Dispose();
			}
		}

		public double FaceCompare(string source, string target) {
			var base64 = new ArrayBase64<double>();
			var sourceFace = base64.FromBase64String(source).ToArray();
			var targetFace = base64.FromBase64String(target).ToArray();

			var sourceFeature = FaceRecognition.LoadFaceEncoding(sourceFace);
			var targetFeature = FaceRecognition.LoadFaceEncoding(targetFace);

			return FaceRecognition.FaceDistance(targetFeature, sourceFeature);
		}

		public IEnumerable<string> GetFaceFeature(string image) {
			var faceLandmarks = LoadFaceLandmarks(image);
			return LoadFaceFeature(faceLandmarks);
		}

		public IEnumerable<string> GetFaceFeature(byte[] image) {
			var faceLandmarks = LoadFaceLandmarks(image);
			return LoadFaceFeature(faceLandmarks);
		}

		public void GetFaceLandmarks(string file) {
			var image = FaceRecognition.LoadImageFile(file);
			var landmarks = _faceRecognition.FaceLandmark(image);

		}

		IEnumerable<string> LoadFaceFeature(Tuple<Image, Location[]> tuple) {
			if (tuple.Item2.Any()) {
				var faces = _faceRecognition.FaceEncodings(tuple.Item1, tuple.Item2, model: _model);
				foreach (var face in faces) {
					var feature = face.GetRawEncoding();
					yield return new ArrayBase64<double>().ToBase64String(feature);
				}
			}
		}

		Tuple<Image, Location[]> LoadFaceLandmarks(string file) {
			var image     = FaceRecognition.LoadImageFile(file);
			return GetFaces(image);
		}

		Tuple<Image, Location[]> LoadFaceLandmarks(byte[] data) {
			using var mat = Cv2.ImDecode(data, ImreadModes.Color);
			var bytes = new byte[mat.Rows * mat.Cols * mat.ElemSize()];
			var image = FaceRecognition.LoadImage(bytes, mat.Rows, mat.Cols, mat.ElemSize(), Mode.Rgb);
			return GetFaces(image);
		}

		Tuple<Image, Location[]> GetFaces(Image image) {
			var locations = _faceRecognition.FaceLocations(image, model: _model).ToArray();
			return new Tuple<Image, Location[]>(image, locations);
		}
	}
}