using Magicube.Data.Abstractions.SqlBuilder;
using Magicube.Data.Abstractions.SqlBuilder.Operators;

namespace Magicube.Data.SqlServer {
    public class SqlServerBuilder : DefaultSqlBuilder<SqlServerCompiler> {
        public SqlServerBuilder(DbOperator<SqlServerCompiler> @operator) : base(@operator) {
        }
    }
}
