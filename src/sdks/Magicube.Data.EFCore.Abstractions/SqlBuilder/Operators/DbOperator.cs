namespace Magicube.Data.Abstractions.SqlBuilder.Operators {
    public class DbOperator<T> where T : SqlCompiler {
        public T SqlCompiler { get; }

        public DbOperator(T compiler) {
            SqlCompiler      = compiler;
        }

        public SqlResult CreateDatabase(string dbName) {
            return SqlCompiler.CreateDatabase(dbName);
        }

        public SqlResult GetTableSchemas(string dbName) {
            var @operator = new OperatorContext<BuildSchemaOperator>(new BuildSchemaOperator(dbName, SqlCompiler));
            return @operator.Build();
        }

        public OperatorContext<AlterTableOperator> AlterTable(string tbName) {
            return new OperatorContext<AlterTableOperator>(new AlterTableOperator(tbName, SqlCompiler));
        }
        public OperatorContext<CreateSchemaOperator> CreateTable(string tbName) {
            return new OperatorContext<CreateSchemaOperator>(new CreateSchemaOperator(tbName, SqlCompiler));
        }
        public OperatorContext<DropSchemaOperator> DropTable(string tbName) {
            return new OperatorContext<DropSchemaOperator>(new DropSchemaOperator(tbName, SqlCompiler));
        }
        public OperatorContext<DeleteOperator> Delete(string tbName) {
            return new OperatorContext<DeleteOperator>(new DeleteOperator(tbName, SqlCompiler));
        }
        public OperatorContext<InsertOperator> Insert(string tbName) {
            return new OperatorContext<InsertOperator>(new InsertOperator(tbName, SqlCompiler));
        }
        public QueryOperator Query(string tbName) {
            return new QueryOperator(tbName, SqlCompiler);
        }
        public OperatorContext<UpdateOperator> Update(string tbName) {
            return new OperatorContext<UpdateOperator>(new UpdateOperator(tbName, SqlCompiler));
        }
    }
}
