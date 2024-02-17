using System;
using System.Collections.Generic;

namespace Magicube.AI.Faces {
	public interface IFaceRecognizeProvider : IDisposable {
		/// <summary>
		/// 返回检测到的多个人脸特征
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		IEnumerable<string> GetFaceFeature(string path);

		/// <summary>
		/// 返回检测到的多个人脸特征
		/// </summary>
		/// <param name="datas"></param>
		/// <returns></returns>
		IEnumerable<string> GetFaceFeature(byte[] datas);

		/// <summary>
		/// 对比人脸
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		double FaceCompare(string source, string target);
	}
}