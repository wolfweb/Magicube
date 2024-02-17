using Magicube.Data.Abstractions.SqlBuilder.Clauses;
using System.Collections.Generic;

namespace Magicube.Data.Abstractions.SqlBuilder.Operators {
    public class CreateSchemaOperator : AbstractOperator {
        public CreateSchemaOperator(string tbName, SqlCompiler compiler) : base(compiler) {
            TableName = tbName;
        }

        public override string Action => "create table";

        protected override string[] BuildStructure(SqlResult sql) {
            return new[] { 
                Action,
                _compiler.WrapKey(TableName),
                "(",
                BuildColumns(),
                ");"
            };
        }

        private string BuildColumns() {
            var columns = GetClause(x => x.GetType() == typeof(ColumnsClause)) as ColumnsClause;
            var fields = new List<string>();
            foreach(var column in columns.Columns) {
                fields.Add(_compiler.BuildCreateColumn(column));
            }
            return string.Join(",",fields);
        }
    }
}
