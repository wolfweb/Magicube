namespace Magicube.Data.Abstractions.SqlBuilder.Operators {
    public class QueryOperator : SqlKata.Query {
        private readonly SqlCompiler _compiler;
        public QueryOperator(string tbName, SqlCompiler compiler) : base(tbName) {
            _compiler = compiler;
        }

        public SqlResult Build() {
            var result = _compiler.Compiler.Compile(this);
            var sqlResult = new SqlResult {
                Sql            = result.Sql,
                ParamsBindings = result.NamedBindings
            };
            return sqlResult;
        }
    }
}
