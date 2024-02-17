using Magicube.Data.Abstractions.SqlBuilder.Clauses;
using System;
using System.Collections.Generic;

namespace Magicube.Data.Abstractions.SqlBuilder.Operators {
    public class InsertOperator : AbstractOperator {
        public InsertOperator(string tbName, SqlCompiler compiler) : base(compiler) {
            TableName = tbName;
        }
        public bool ReturnId  { get; set; } = true;

        public override string Action => "insert into";

        protected override string[] BuildStructure(SqlResult sql) {
            var clause = GetClause(x => typeof(InsertClause).IsAssignableFrom(x.GetType())) as InsertClause;
            var args = new List<string>();
            foreach (var it in clause.Values) {
                var k = $"{_compiler.Slim}{sql.ParamsBindings.Count}";
                sql.ParamsBindings.Add(k, it);
                args.Add(k);
            }
            return new[] {
                Action,
                _compiler.WrapKey(TableName),
                $"({string.Join(",", clause.Columns)})",
                "values",
                $"({string.Join(",",args)})",
                ReturnId ? $";{_compiler.ReturnLastId}" : null
            };
        }
    }
}
