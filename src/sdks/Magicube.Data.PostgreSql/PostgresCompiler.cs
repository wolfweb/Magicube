using Magicube.Data.Abstractions.SqlBuilder;
using Magicube.Data.Abstractions.SqlBuilder.Clauses;
using System;
using System.Collections.Generic;

namespace Magicube.Data.PostgreSql {
    public class PostgresCompiler : SqlCompiler {
        public PostgresCompiler(){
            CompleteTypeMappings();
        }
        public override string ReturnLastId => "SELECT lastval() AS Id";

        public override SqlKata.Compilers.Compiler Compiler => new SqlKata.Compilers.PostgresCompiler();

        public override string BuildCreateColumn(ColumnItem column) {
            var type = GetDbType(column);
            
            var builder = new List<string>();

            builder.Add($"{WrapKey(column.Name)}");

            if (column.AutoIncrement) {
                builder.Add("serial");
            } else { 
               builder.Add(type);
            }

            if (column.PrimaryKey)
                builder.Add("primary key");

            if (column.UniqueKey)
                builder.Add("unique");
            
            if (column.Nullable && !column.AutoIncrement) {
                builder.Add("default null");
            } else {
                builder.Add("not null");
            }

            return string.Join(" ", builder);
        }

        public override string[] BuildTableSchemas(string dbName) {
            return new[] {
                "cast(ptables.tablename as varchar) as TableName,",
                "pcolumn.column_name as ColumnName,",
                "pcolumn.udt_name as DataType,",
                "case when pcolumn.numeric_scale >0 then pcolumn.numeric_precision else pcolumn.character_maximum_length end as Length,",
                "pcolumn.column_default as DefaultValue,",
                "col_description(pclass.oid, pcolumn.ordinal_position) as ColumnDescription,",
                "case when pkey.colname = pcolumn.column_name then true else false end as IsPrimaryKey,",
                "case when pcolumn.column_default like 'nextval%' then true else false end as IsIdentity,",
                "case when pcolumn.is_nullable = 'YES' then true else false end as IsNullable",
                "from",
                "(select * from pg_tables where schemaname='public') ptables",
                "inner join pg_class pclass on ptables.tablename = pclass.relname",
                "inner join (SELECT * FROM information_schema.columns) pcolumn on pcolumn.table_name = ptables.tablename",
                "left join (",
                "select  pg_class.relname,pg_attribute.attname as colname from pg_constraint",
                "inner join pg_class on pg_constraint.conrelid = pg_class.oid",
                "inner join pg_attribute on pg_attribute.attrelid = pg_class.oid and  pg_attribute.attnum = pg_constraint.conkey[1]",
                "inner join pg_type on pg_type.oid = pg_attribute.atttypid",
                "where pg_constraint.contype='p'",
                ")",
                "pkey on pcolumn.table_name = pkey.relname",
                "order by ptables.tablename"
            };
        }

        public override string GetDbType(ColumnItem column) {
            var builder = new List<string>();
            var type = base.GetDbType(column);

            if (column.Size > 0) {
                if (column.Size < 4000)
                    builder.Add($"{type}({column.Size})");
                else
                    builder.Add("text");
            } else {
                builder .Add(type);
            }
            return string.Join(" ", builder);
        }

        private void CompleteTypeMappings() {
            TypeMappings.Clear();

            TypeMappings.AddRange(new[] {
                 new KeyValuePair<string, Type>("int2"                       ,typeof(short)),
                 new KeyValuePair<string, Type>("smallint"                   ,typeof(short)),
                 new KeyValuePair<string, Type>("int4"                       ,typeof(int)),
                 new KeyValuePair<string, Type>("integer"                    ,typeof(int)),
                 new KeyValuePair<string, Type>("int8"                       ,typeof(long)),
                 new KeyValuePair<string, Type>("bigint"                     ,typeof(long)),
                 new KeyValuePair<string, Type>("float4"                     ,typeof(float)),
                 new KeyValuePair<string, Type>("real"                       ,typeof(float)),
                 new KeyValuePair<string, Type>("float8"                     ,typeof(double)),
                 new KeyValuePair<string, Type>("double precision"           ,typeof(int)),
                 new KeyValuePair<string, Type>("numeric"                    ,typeof(decimal)),
                 new KeyValuePair<string, Type>("decimal"                    ,typeof(decimal)),
                 new KeyValuePair<string, Type>("path"                       ,typeof(decimal)),
                 new KeyValuePair<string, Type>("point"                      ,typeof(decimal)),
                 new KeyValuePair<string, Type>("polygon"                    ,typeof(decimal)),
                 new KeyValuePair<string, Type>("boolean"                    ,typeof(bool)),
                 new KeyValuePair<string, Type>("bool"                       ,typeof(bool)),
                 new KeyValuePair<string, Type>("box"                        ,typeof(bool)),
                 new KeyValuePair<string, Type>("bytea"                      ,typeof(bool)),
                 new KeyValuePair<string, Type>("varchar"                    ,typeof(string)),
                 new KeyValuePair<string, Type>("character varying"          ,typeof(string)),
                 new KeyValuePair<string, Type>("name"                       ,typeof(string)),
                 new KeyValuePair<string, Type>("text"                       ,typeof(string)),
                 new KeyValuePair<string, Type>("char"                       ,typeof(string)),
                 new KeyValuePair<string, Type>("character"                  ,typeof(string)),
                 new KeyValuePair<string, Type>("cidr"                       ,typeof(string)),
                 new KeyValuePair<string, Type>("circle"                     ,typeof(string)),
                 new KeyValuePair<string, Type>("tsquery"                    ,typeof(string)),
                 new KeyValuePair<string, Type>("tsvector"                   ,typeof(string)),
                 new KeyValuePair<string, Type>("txid_snapshot"              ,typeof(string)),
                 new KeyValuePair<string, Type>("uuid"                       ,typeof(Guid)),
                 new KeyValuePair<string, Type>("xml"                        ,typeof(string)),
                 new KeyValuePair<string, Type>("json"                       ,typeof(string)),
                 new KeyValuePair<string, Type>("interval"                   ,typeof(decimal)),
                 new KeyValuePair<string, Type>("lseg"                       ,typeof(decimal)),
                 new KeyValuePair<string, Type>("macaddr"                    ,typeof(decimal)),
                 new KeyValuePair<string, Type>("money"                      ,typeof(decimal)),
                 new KeyValuePair<string, Type>("timestamp"                  ,typeof(DateTime)),
                 new KeyValuePair<string, Type>("timestamp with time zone"   ,typeof(DateTime)),
                 new KeyValuePair<string, Type>("timestamptz"                ,typeof(DateTime)),
                 new KeyValuePair<string, Type>("timestamp without time zone",typeof(DateTime)),
                 new KeyValuePair<string, Type>("date"                       ,typeof(DateTime)),
                 new KeyValuePair<string, Type>("time"                       ,typeof(DateTime)),
                 new KeyValuePair<string, Type>("time with time zone"        ,typeof(DateTime)),
                 new KeyValuePair<string, Type>("timetz"                     ,typeof(DateTime)),
                 new KeyValuePair<string, Type>("time without time zone"     ,typeof(DateTime)),
                 new KeyValuePair<string, Type>("bit"                        ,typeof(byte[])),
                 new KeyValuePair<string, Type>("bit varying"                ,typeof(byte[])),
                 new KeyValuePair<string, Type>("varbit"                     ,typeof(byte))
            });
        }
    }
}
