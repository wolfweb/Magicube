using Aliyun.OSS;
using Magicube.Storage.Abstractions.Entities;
using Magicube.Storage.Abstractions.Models;
using Magicube.Storage.Abstractions.Services;
using Magicube.Storage.Abstractions.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Magicube.Storage.Aliyun.Services {
	public class AliyunStorageService : IStorageSignatureService {
		private readonly ConcurrentDictionary<string, OssClient> _ossClients = new ConcurrentDictionary<string, OssClient>();
		private readonly IStoragePathGenerator _storagePathGenerator;
		public AliyunStorageService(IStoragePathGenerator storagePathGenerator ) {
            _storagePathGenerator = storagePathGenerator;
        }

		public SignatureViewModel GenerateWebUploadSignature(string uploadUrl, SignatureModel model, StorageStore storage) {
            var attr   = storage.Attribute.ToObject<CloudStorageStore>();
            var client = _ossClients.GetOrAdd(attr.Bucket, new OssClient(attr.EndPoint, attr.AccessKey, attr.AccessSecret));
			var path   = _storagePathGenerator.Generate(storage.Template, model.Path);
            

            var fileKey          = path.TrimStart('/');
			var expiration       = DateTime.Now.AddMinutes(15);
			var policyConditions = new PolicyConditions();
			policyConditions.AddConditionItem(PolicyConditions.CondContentLengthRange, 1, storage.MaxSize * 1024 * 1024);
			policyConditions.AddConditionItem("bucket", attr.Bucket);
			policyConditions.AddConditionItem("key", fileKey);

			var postPolicy = client.GeneratePostPolicy(expiration, policyConditions);
			var encPolicy  = Convert.ToBase64String(Encoding.Default.GetBytes(postPolicy));
			var signature  = ComputeSignature(attr.AccessSecret, encPolicy);

			return new SignatureViewModel {
				Url      = $"https://{attr.Bucket}.{attr.EndPoint}",
				Name     = "file",
				Headers  = new Dictionary<string, string> { },
				FormData = new Dictionary<string, string> {
					["key"]                   = fileKey,
					["policy"]                = encPolicy,
					["signature"]             = signature,
					["OSSAccessKeyId"]        = attr.AccessKey,
					["success_action_status"] = "200",
				},
				SuccessUrl = $"http://{attr.Host}/{fileKey}"
			};
		}

		private string ComputeSignature(string key, string data) {
			using var algorithm = KeyedHashAlgorithm.Create("HmacSHA1");
			algorithm.Key = Encoding.ASCII.GetBytes(key.ToCharArray());
			return Convert.ToBase64String(algorithm.ComputeHash(Encoding.ASCII.GetBytes(data.ToCharArray())));
		}
	}
}
