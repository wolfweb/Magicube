using Magicube.Data.Abstractions.SqlBuilder.Clauses;
using System;

namespace Magicube.Data.Abstractions.SqlBuilder.Operators {
    public class WhereOperatorContext<TO> : OperatorContext<TO> where TO : AbstractOperator {
        public WhereOperatorContext(OperatorContext<TO> ctx) : base(ctx.Operator) { }

        public void AppendClauseBy(Func<Clause, bool> condition, Func<Clause, Clause> callback) {
            var clause = Operator.GetLastClause(condition);
            Operator.AppendClause(callback(clause));
        }
    }
}
