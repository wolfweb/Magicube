using System;

namespace Magicube.Versioning.Abstractions {
	public interface IVersioningContent {
		public string    Key      { get; set; }
		public string    Content  { get; set; }
		public DateTime  CreateAt { get; set; }
		public DateTime? UpdateAt { get; set; }
	}
}