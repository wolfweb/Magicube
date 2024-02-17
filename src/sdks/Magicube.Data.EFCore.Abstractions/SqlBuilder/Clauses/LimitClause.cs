namespace Magicube.Data.Abstractions.SqlBuilder.Clauses {
    public class LimitClause : Clause {
        public LimitClause(int count) {
            Count = count;
        }
        public int Count { get; }
    }
}
