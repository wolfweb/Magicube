using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magicube.Versioning.Abstractions {
    public interface IVersioningProvider {
		void AddOrUpdate(IVersioningContent content);
		VersioningContentChanges Compare(VersioningHistory v1, VersioningHistory v2);
		string GetVersionContent(VersioningHistory history);
		IEnumerable<VersioningHistory> Query(IVersioningContent content);
		void RecoveryTo(VersioningHistory history);
		void Remove(IVersioningContent content);
		void SyncToRemote();
		void SyncFromRemote();
    }	
}