using System;
using System.Linq;

namespace Magicube.Data.Abstractions.SqlBuilder.Operators {
    public class BuildSchemaOperator : AbstractOperator {
        private readonly string _dbName;
        public BuildSchemaOperator(string dbName, SqlCompiler compiler) : base(compiler) {
            _dbName = dbName;
        }

        public override string Action => "select";

        protected override string[] BuildStructure(SqlResult sql) {
            return new[] {
                Action,                
            }.Concat(_compiler.BuildTableSchemas(_dbName)).ToArray();
        }
    }
}
