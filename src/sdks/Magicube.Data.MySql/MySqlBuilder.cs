using Magicube.Data.Abstractions.SqlBuilder;
using Magicube.Data.Abstractions.SqlBuilder.Operators;

namespace Magicube.Data.MySql {
    public class MySqlBuilder : DefaultSqlBuilder<MySqlCompiler> {
        public MySqlBuilder(DbOperator<MySqlCompiler> @operator) : base(@operator) {
            
        }
    }
}
