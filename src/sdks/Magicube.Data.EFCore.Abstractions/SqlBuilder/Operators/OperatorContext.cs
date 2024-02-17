using Magicube.Data.Abstractions.SqlBuilder.Clauses;

namespace Magicube.Data.Abstractions.SqlBuilder.Operators {
    public class OperatorContext<TO> where TO : AbstractOperator {
        internal OperatorContext(TO @operator) {
            Operator = @operator;
        }
        public TO Operator { get; }

        public void AppendClause(Clause clause) {
            Operator.AppendClause(clause);
        }

        public SqlResult Build() {
            return Operator.Compile();
        }
    }
}
