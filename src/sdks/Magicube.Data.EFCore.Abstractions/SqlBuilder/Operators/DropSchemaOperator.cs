namespace Magicube.Data.Abstractions.SqlBuilder.Operators {
    public class DropSchemaOperator : AbstractOperator {
        public DropSchemaOperator(string tbName, SqlCompiler compiler) : base(compiler) {
            TableName = tbName;
        }

        public override string Action => "drop table";

        protected override string[] BuildStructure(SqlResult sql) {
            return new[] {
               Action,
               _compiler.WrapKey(TableName)
            };
        }
    }
}
