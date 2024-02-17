using System.Collections.Generic;

namespace Magicube.Versioning.Abstractions {
    public record struct ChangeLine(int Line, string Content);

    public class VersioningContentChanges {
		public VersioningContentChanges(int linesAdded, int linesDeleted) {
			LinesAdded   = linesAdded;
			LinesDeleted = linesDeleted;
		}
        public int              LinesAdded   { get; }
		public int              LinesDeleted { get; }
        public List<ChangeLine> AddedLines   { get; } = new List<ChangeLine>();
        public List<ChangeLine> DeletedLines { get; } = new List<ChangeLine>();
    }
}