using Magicube.Data.Abstractions.SqlBuilder;
using Magicube.Data.Abstractions.SqlBuilder.Clauses;
using Magicube.Data.Abstractions.SqlBuilder.Operators;
using System;
using System.Collections.Generic;

namespace Magicube.Data.SqlServer {
    public class SqlServerCompiler : SqlCompiler {
        public SqlServerCompiler() {
            CompleteTypeMappings();
        }
        public override string PrefixSymbol => "[";
        public override string SuffixSymbol => "]";
        public override string ReturnLastId => "SELECT scope_identity() as Id";

        public override SqlKata.Compilers.Compiler Compiler => new SqlKata.Compilers.SqlServerCompiler();

        public override string BuildColumns(SqlResult sql, AbstractOperator @operator) {
            var clause = @operator.GetClause(x => x.GetType() == typeof(LimitClause)) as LimitClause;
            var limit = "";
            if (clause != null) {
                var k = $"{Slim}{sql.ParamsBindings.Count}";
                sql.ParamsBindings.Add(k, clause.Count);
                limit = $"top {clause.Count}";
            }
            return $"{limit} {base.BuildColumns(sql, @operator)}";
        }

        public override string BuildLimit(SqlResult sql, LimitClause clause) {
            return null;
        }

        private void CompleteTypeMappings() {
            TypeMappings.AddRange(new[] {
                new KeyValuePair<string, Type>("nvarchar"      ,typeof(string)),
                new KeyValuePair<string, Type>("sql_variant"   ,typeof(string)),
                new KeyValuePair<string, Type>("text"          ,typeof(string)),
                new KeyValuePair<string, Type>("char"          ,typeof(string)),
                new KeyValuePair<string, Type>("ntext"         ,typeof(string)),
                new KeyValuePair<string, Type>("nchar"         ,typeof(string)),
                new KeyValuePair<string, Type>("hierarchyid"   ,typeof(string)),
                new KeyValuePair<string, Type>("time"          ,typeof(DateTime)),
                new KeyValuePair<string, Type>("smalldatetime" ,typeof(DateTime)),
                new KeyValuePair<string, Type>("timestamp"     ,typeof(byte[])),
                new KeyValuePair<string, Type>("datetime2"     ,typeof(DateTime)),
                new KeyValuePair<string, Type>("date"          ,typeof(DateTime)),
                new KeyValuePair<string, Type>("single"        ,typeof(decimal)),
                new KeyValuePair<string, Type>("money"         ,typeof(decimal)),
                new KeyValuePair<string, Type>("numeric"       ,typeof(decimal)),
                new KeyValuePair<string, Type>("smallmoney"    ,typeof(decimal)),
                new KeyValuePair<string, Type>("float"         ,typeof(double)),
                new KeyValuePair<string, Type>("real"          ,typeof(float)),
                new KeyValuePair<string, Type>("image"         ,typeof(byte[])),
                new KeyValuePair<string, Type>("varbinary"     ,typeof(byte[])),
                new KeyValuePair<string, Type>("datetimeoffset",typeof(DateTime))
            });

        }
    }
}
