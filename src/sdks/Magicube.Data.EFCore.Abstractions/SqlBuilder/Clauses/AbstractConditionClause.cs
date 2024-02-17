namespace Magicube.Data.Abstractions.SqlBuilder.Clauses {
    public abstract class AbstractConditionClause : Clause {
        public AbstractConditionClause(int group, int pos) {
            Group = group;
            Pos   = pos;
        }
        public int Group { get; }
        public int Pos   { get; }
    }

    public class ConditionFactorClause : AbstractConditionClause {
        public ConditionFactorClause(string factor, int group, int pos) : base(group, pos) {
            Factor = factor;
        }

        public string Factor { get; }
    }
    public class ConditionClause : AbstractConditionClause {
        public ConditionClause(string column, object v, string ope, int group = 1, int pos = 1) : base(group, pos) {
            Value    = v;
            Column   = column;
            Operator = ope;
        }
        public string Column   { get; }
        public object Value    { get; }
        public string Operator { get; }
        public override string ToString() {
            return $"";
        }
    }
}
