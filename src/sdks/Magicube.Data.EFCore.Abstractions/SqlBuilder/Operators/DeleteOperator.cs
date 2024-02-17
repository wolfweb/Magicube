using System.Linq;

namespace Magicube.Data.Abstractions.SqlBuilder.Operators {
    public class DeleteOperator : AbstractOperator {
        public DeleteOperator(string tbName, SqlCompiler compiler) : base(compiler) {
            TableName = tbName;
        }
        public override string Action => "delete";

        protected override string[] BuildStructure(SqlResult sql) {
            return new[] {
                Action,
                "from",
                _compiler.WrapKey(TableName),
                BuildCondition(sql)
            }.Where(x => x != null).ToArray();
        }
    }
}
