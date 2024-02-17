using Magicube.Data.Abstractions.SqlBuilder;
using Magicube.Data.Abstractions.SqlBuilder.Operators;

namespace Magicube.Data.PostgreSql {
    public class PostgreSqlBuilder : DefaultSqlBuilder<PostgresCompiler> {
        public PostgreSqlBuilder(DbOperator<PostgresCompiler> @operator) : base(@operator) {
        }
    }
}
