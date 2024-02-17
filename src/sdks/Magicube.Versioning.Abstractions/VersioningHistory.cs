using System;

namespace Magicube.Versioning.Abstractions {
	public class VersioningHistory {
		public string   Key                   { get; set; }
		public string   Desc                  { get; set; }
		public DateTime At                    { get; set; }
		public string   VersioningId          { get; set; }
		public string   VersioningContentId   { get; set; }
	}
}