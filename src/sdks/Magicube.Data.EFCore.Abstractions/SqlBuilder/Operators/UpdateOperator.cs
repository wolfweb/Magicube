using Magicube.Data.Abstractions.SqlBuilder.Clauses;
using System;
using System.Collections.Generic;

namespace Magicube.Data.Abstractions.SqlBuilder.Operators {
    public class UpdateOperator : AbstractOperator {
        public UpdateOperator(string tbName, SqlCompiler compiler) : base(compiler) {
            TableName = tbName;
        }
        public override string Action => "update";

        protected override string[] BuildStructure(SqlResult sql) {
            return new[] {
               Action,
               _compiler.WrapKey(TableName),
               "set",
               BuildUpdater(sql),
               BuildCondition(sql)
            };
        }

        private string BuildUpdater(SqlResult sql) {
            var clause = GetClause(x => typeof(UpdatesClause).IsAssignableFrom(x.GetType())) as UpdatesClause;
            var args = new List<string>();
            foreach (var item in clause.Updates) {
                var k = $"{_compiler.Slim}{sql.ParamsBindings.Count}";
                args.Add($"{_compiler.WrapKey(item.Column)} = {k}");
                sql.ParamsBindings.Add(k, item.Value);
            }
            return string.Join(",", args);
        }
    }
}
