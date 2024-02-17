using Magicube.Data.Abstractions.SqlBuilder;
using Magicube.Data.Abstractions.SqlBuilder.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.Data.MySql {
    public class MySqlCompiler : SqlCompiler {
        public MySqlCompiler() {
            CompleteTypeMappings();
        }
        public override string PrefixSymbol => "`";
        public override string SuffixSymbol => "`";
        public override string ReturnLastId => "select last_insert_id() as Id;";

        public override SqlKata.Compilers.Compiler Compiler => new SqlKata.Compilers.MySqlCompiler();

        public override string BuildCreateColumn(ColumnItem column) {
            var type = GetDbType(column);
            var builder = new List<string>();
            builder.Add($"{WrapKey(column.Name)}");

            builder.Add(type);

            if (column.PrimaryKey)
                builder.Add("primary key");

            if (column.UniqueKey)
                builder.Add("unique key");

            if (column.AutoIncrement)
                builder.Add("auto_increment");
            else {
                if (column.Nullable) {
                    builder.Add("default null");
                } else {
                    builder.Add("not null");
                }
            }

            return string.Join(" ", builder);
        }
        public override string[] BuildTableSchemas(string dbName) {
            return new[] {
               "table_name as TableName,",
                "column_name as ColumnName,",
                "case when left(column_type,locate('(',column_type)-1)='' then column_type else left(column_type,locate('(',column_type)-1) end as DataType,",
                "cast(substring(column_type,locate('(',column_type)+1,locate(')',column_type)-locate('(',column_type)-1) as signed) as Length,",
                "column_default as DefaultValue,",
                "case when column_key = 'PRI' then true else false end as IsPrimaryKey,",
                "case when column_key = 'UNI' then true else false end as UniqueKey,",
                "case when extra = 'auto_increment' then true else false end as AutoIncrement,",
                "case when is_nullable = 'YES' then true else false end as Nullable",
                "from",
                $"information_schema.columns where table_schema='{dbName}' and table_schema=(select database())"
            };
        }

        public override string GetDbType(ColumnItem column) {
            var builder = new List<string>();
            var type = base.GetDbType(column);

            if (column.Size > 0) {
                if (column.Size < 4000)
                    builder.Add($"{type}({column.Size})");
                else if (column.Size > 4000 && column.Size < 65535)
                    builder.Add($"text");
                else
                    builder.Add($"mediumtext");
            } else {
                builder.Add($"{type}");
            }
            return string.Join(" ", builder);
        }

        private void CompleteTypeMappings() {
            var guid = TypeMappings.FirstOrDefault(x => x.Value == typeof(Guid));
            if (guid.Value != null) {
                TypeMappings.Remove(guid);
            }

            TypeMappings.AddRange(new[] {
                new KeyValuePair<string, Type>("varchar"        ,typeof(Guid)    ),
                new KeyValuePair<string, Type>("mediumint"      ,typeof(int)     ),
                new KeyValuePair<string, Type>("integer"        ,typeof(int)     ),
                new KeyValuePair<string, Type>("text"           ,typeof(string)  ),
                new KeyValuePair<string, Type>("char"           ,typeof(string)  ),
                new KeyValuePair<string, Type>("enum"           ,typeof(string)  ),
                new KeyValuePair<string, Type>("mediumtext"     ,typeof(string)  ),
                new KeyValuePair<string, Type>("tinytext"       ,typeof(string)  ),
                new KeyValuePair<string, Type>("longtext"       ,typeof(string)  ),
                new KeyValuePair<string, Type>("real"           ,typeof(double)  ),
                new KeyValuePair<string, Type>("float"          ,typeof(float)   ),
                new KeyValuePair<string, Type>("numeric"        ,typeof(decimal) ),
                new KeyValuePair<string, Type>("year"           ,typeof(int)     ),
                new KeyValuePair<string, Type>("timestamp"      ,typeof(DateTime)),
                new KeyValuePair<string, Type>("date"           ,typeof(DateTime)),
                new KeyValuePair<string, Type>("time"           ,typeof(DateTime)),
                new KeyValuePair<string, Type>("blob"           ,typeof(byte[])  ),
                new KeyValuePair<string, Type>("longblob"       ,typeof(byte[])  ),
                new KeyValuePair<string, Type>("tinyblob"       ,typeof(byte[])  ),
                new KeyValuePair<string, Type>("varbinary"      ,typeof(byte[])  ),
                new KeyValuePair<string, Type>("multipoint"     ,typeof(byte[])  ),
                new KeyValuePair<string, Type>("geometry"       ,typeof(byte[])  ),
                new KeyValuePair<string, Type>("multilinestring",typeof(byte[])  ),
                new KeyValuePair<string, Type>("polygon"        ,typeof(byte[])  ),
                new KeyValuePair<string, Type>("mediumblob"     ,typeof(byte[])  )
            });                
        }
    }
}
