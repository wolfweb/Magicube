using Magicube.Data.Abstractions.SqlBuilder.Operators;

namespace Magicube.Data.Abstractions.SqlBuilder {
    public interface ISqlBuilder {
        SqlCompiler SqlCompiler { get; }

        SqlResult CreateDatabase(string dbName);
        SqlResult GetTableSchemas(string dbName);
        OperatorContext<AlterTableOperator>   AlterTable(string tbName);
        OperatorContext<CreateSchemaOperator> CreateTable(string tbName);
        OperatorContext<DropSchemaOperator>   DropTable(string tbName);
        OperatorContext<DeleteOperator>       Delete(string tbName);
        OperatorContext<InsertOperator>       Insert(string tbName);
        OperatorContext<UpdateOperator>       Update(string tbName);
        QueryOperator                         Query(string tbName);
    }

    public abstract class DefaultSqlBuilder<TSqlCompiler> : ISqlBuilder where TSqlCompiler : SqlCompiler {
        protected readonly DbOperator<TSqlCompiler> _operator;
        public DefaultSqlBuilder(DbOperator<TSqlCompiler> @operator) {
            _operator = @operator;
        }

        public SqlCompiler SqlCompiler => _operator.SqlCompiler;

        public SqlResult GetTableSchemas(string dbName) {
            return _operator.GetTableSchemas(dbName);
        }

        public SqlResult CreateDatabase(string dbName) {
            return _operator.CreateDatabase(dbName);
        }

        public OperatorContext<AlterTableOperator> AlterTable(string tbName) {
            return _operator.AlterTable(tbName);
        }

        public OperatorContext<CreateSchemaOperator> CreateTable(string tbName) {
            return _operator.CreateTable(tbName);
        }

        public OperatorContext<DeleteOperator> Delete(string tbName) {
            return _operator.Delete(tbName);
        }

        public OperatorContext<DropSchemaOperator> DropTable(string tbName) {
            return _operator.DropTable(tbName);
        }

        public OperatorContext<InsertOperator> Insert(string tbName) {
            return _operator.Insert(tbName);
        }

        public QueryOperator Query(string tbName) {
            return _operator.Query(tbName);
        }

        public OperatorContext<UpdateOperator> Update(string tbName) {
            return _operator.Update(tbName);
        }
    }
}
