using Magicube.Data.Abstractions.SqlBuilder.Clauses;

namespace Magicube.Data.Abstractions.SqlBuilder.Operators {
    public class AlterTableOperator : AbstractOperator {
        public AlterTableOperator(string tbName, SqlCompiler compiler) : base(compiler) {
            TableName = tbName;
        }

        public override string Action => "alter";

        protected override string[] BuildStructure(SqlResult sql) {
            var clause = GetClause(x => typeof(AlterClause).IsAssignableFrom(x.GetType())) as AlterClause;

            return new[] {
                Action,
                "table",
                _compiler.WrapKey(TableName),
                _compiler.BuildAlterColumn(clause),
                ";"
            };
        }
    }
}
