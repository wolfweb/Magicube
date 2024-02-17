using Magicube.Data.Abstractions.SqlBuilder.Clauses;
using Magicube.Data.Abstractions.SqlBuilder.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using Magicube.Core;

namespace Magicube.Data.Abstractions.SqlBuilder {
    public abstract class SqlCompiler {
        public virtual string PrefixSymbol { get; } = "\"";
        public virtual string SuffixSymbol { get; } = "\"";
        public virtual string ReturnLastId { get; } = "";
        public string  Slim { get; } = "@";

        public abstract SqlKata.Compilers.Compiler Compiler { get; }

        public virtual SqlResult CreateDatabase(string dbName) {
            if (dbName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(dbName));
            var sql = new SqlResult() { 
              Sql = $"create database {dbName}"
            };

            return sql;
        }

        public virtual string BuildColumns(SqlResult sql, AbstractOperator @operator) {
            var clause = @operator.GetClause(x => x.GetType() == typeof(ColumnsClause)) as ColumnsClause;
            if (clause == null) return "*";
            return string.Join(",", clause.Columns.Select(x => WrapKey(x.Name)));
        }

        public virtual string BuildLimit(SqlResult sql, LimitClause clause) {
            var key = $"{Slim}{sql.ParamsBindings.Values.Count}";
            sql.ParamsBindings.Add(key, clause.Count);
            return $"limit {key}";
        }

        public virtual string BuildAlterColumn(AlterClause clause) {
            var builder = new List<string>();
            builder.Add(clause.Action);
            if(clause.Action == "add") {
                builder.Add(WrapKey(clause.Column.Name));
                var type = GetDbType(clause.Column);
                builder.Add(type);
            } else if(clause.Action == "drop") {
                builder.Add("column");
                builder.Add(WrapKey(clause.Column.Name));
            }

            return string.Join(" ", builder);
        }

        public virtual string BuildCreateColumn(ColumnItem column) {
            return string.Empty;
        }

        public virtual string[] BuildTableSchemas(string dbName) {
            return Array.Empty<string>();
        }
        
        public virtual string GetDbType(ColumnItem column) {
            var mapping = TypeMappings.FirstOrDefault(x => x.Value == column.Type);
            return mapping.Key != null ? mapping.Key : "varchar";
        }

        public Type GetEntityType(string dbType) {
            var mapping = TypeMappings.FirstOrDefault(x => x.Key == dbType);
            return mapping.Key != null ? mapping.Value : typeof(string);
        }

        public string WrapKey(string k) => $"{PrefixSymbol}{k}{SuffixSymbol}";

        protected List<KeyValuePair<string, Type>> TypeMappings = new List<KeyValuePair<string, Type>> {
            new KeyValuePair<string,Type>("int"             ,typeof(int)           ),
            new KeyValuePair<string,Type>("bit"             ,typeof(bool)          ),
            new KeyValuePair<string,Type>("bigint"          ,typeof(long)          ),
            new KeyValuePair<string,Type>("tinyint"         ,typeof(byte)          ),
            new KeyValuePair<string,Type>("uniqueidentifier",typeof(Guid)          ),
            new KeyValuePair<string,Type>("float"           ,typeof(float)         ),
            new KeyValuePair<string,Type>("smallint"        ,typeof(short)         ),
            new KeyValuePair<string,Type>("binary"          ,typeof(byte[])        ), 
            new KeyValuePair<string,Type>("varchar"         ,typeof(string)        ),
            new KeyValuePair<string,Type>("double"          ,typeof(double)        ),
            new KeyValuePair<string,Type>("decimal"         ,typeof(decimal)       ),
            new KeyValuePair<string,Type>("datetime"        ,typeof(DateTime)      ),
            new KeyValuePair<string,Type>("datetimeoffset"  ,typeof(DateTimeOffset))
        };
    }
}
