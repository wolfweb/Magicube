using Magicube.Data.Abstractions.SqlBuilder.Clauses;

namespace Magicube.Data.Abstractions.SqlBuilder.Operators {
    public static class UpdateOperatorContextExtension {
        public static UpdateOperatorContext Set(this OperatorContext<UpdateOperator> ctx, string column, object value) {
            ctx.Operator.AddOrUpdateClause(x => typeof(UpdatesClause).IsAssignableFrom(x.GetType()), x => {
                var clause = x as UpdatesClause ?? new UpdatesClause();
                clause.Updates.Add(new UpdateItem {
                    Column = column,
                    Value = value
                });
                return clause;
            });
            return new UpdateOperatorContext(ctx.Operator);
        }
    }
}
