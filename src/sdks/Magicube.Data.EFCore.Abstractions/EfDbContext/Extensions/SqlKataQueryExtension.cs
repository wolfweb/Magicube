using Magicube.Data.Abstractions.SqlBuilder;
using Magicube.Data.Abstractions.SqlBuilder.Operators;

namespace Magicube.Data.Abstractions.EfDbContext {
    public static class SqlKataQueryExtension {
        public static SqlKata.Query WhereGt(this SqlKata.Query query, string column, object value) {
            query.Where(column, ">", value);
            return query;
        }

        public static SqlKata.Query WhereGte(this SqlKata.Query query, string column, object value) {
            query.Where(column, ">=", value);
            return query;
        }

        public static SqlKata.Query WhereLt(this SqlKata.Query query, string column, object value) {
            query.Where(column, "<", value);
            return query;
        }

        public static SqlKata.Query WhereLte(this SqlKata.Query query, string column, object value) {
            query.Where(column, "<=", value);
            return query;
        }

        public static SqlResult Build(this SqlKata.Query query) {
            if (query is QueryOperator @operator) {
                return @operator.Build();
            }
            throw new DataException($"需要 QueryOperator 来操作");
        }
    }
}
