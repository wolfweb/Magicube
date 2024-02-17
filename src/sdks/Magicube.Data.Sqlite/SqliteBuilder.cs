using Magicube.Data.Abstractions.SqlBuilder;
using Magicube.Data.Abstractions.SqlBuilder.Operators;

namespace Magicube.Data.Sqlite {
    public class SqliteBuilder : DefaultSqlBuilder<SqliteCompiler> {
        public SqliteBuilder(DbOperator<SqliteCompiler> @operator) : base(@operator) {            
        }
    }
}
