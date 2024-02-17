using Magicube.Data.Abstractions.SqlBuilder;
using System;
using System.Collections.Generic;

namespace Magicube.Data.Sqlite {
    public class SqliteCompiler : SqlCompiler {
        public SqliteCompiler() {
            CompleteTypeMappings();
        }
        public override string ReturnLastId => "select last_insert_rowid() as Id";

        public override SqlKata.Compilers.Compiler Compiler => new SqlKata.Compilers.SqliteCompiler();

        private void CompleteTypeMappings() {
            TypeMappings.AddRange(new[]{
                new KeyValuePair<string, Type>("integer"         ,typeof(int)),
                new KeyValuePair<string, Type>("int32"           ,typeof(int)),
                new KeyValuePair<string, Type>("integer32"       ,typeof(int)),
                new KeyValuePair<string, Type>("number"          ,typeof(int)),
                new KeyValuePair<string, Type>("varchar2"        ,typeof(string)),
                new KeyValuePair<string, Type>("nvarchar"        ,typeof(string)),
                new KeyValuePair<string, Type>("nvarchar2"       ,typeof(string)),
                new KeyValuePair<string, Type>("text"            ,typeof(string)),
                new KeyValuePair<string, Type>("ntext"           ,typeof(string)),
                new KeyValuePair<string, Type>("blob_text"       ,typeof(string)),
                new KeyValuePair<string, Type>("char"            ,typeof(string)),
                new KeyValuePair<string, Type>("nchar"           ,typeof(string)),
                new KeyValuePair<string, Type>("num"             ,typeof(string)),
                new KeyValuePair<string, Type>("currency"        ,typeof(string)),
                new KeyValuePair<string, Type>("datetext"        ,typeof(string)),
                new KeyValuePair<string, Type>("word"            ,typeof(string)),
                new KeyValuePair<string, Type>("graphic"         ,typeof(string)),
                new KeyValuePair<string, Type>("unsignedinteger8",typeof(byte)),
                new KeyValuePair<string, Type>("int16"           ,typeof(short)),
                new KeyValuePair<string, Type>("int64"           ,typeof(long)),
                new KeyValuePair<string, Type>("long"            ,typeof(long)),
                new KeyValuePair<string, Type>("integer64"       ,typeof(long)),
                new KeyValuePair<string, Type>("bool"            ,typeof(bool)),
                new KeyValuePair<string, Type>("boolean"         ,typeof(bool)),
                new KeyValuePair<string, Type>("real"            ,typeof(double)),
                new KeyValuePair<string, Type>("dec"             ,typeof(decimal)),
                new KeyValuePair<string, Type>("numeric"         ,typeof(decimal)),
                new KeyValuePair<string, Type>("money"           ,typeof(decimal)),
                new KeyValuePair<string, Type>("smallmoney"      ,typeof(decimal)),
                new KeyValuePair<string, Type>("timestamp"       ,typeof(DateTime)),
                new KeyValuePair<string, Type>("date"            ,typeof(DateTime)),
                new KeyValuePair<string, Type>("time"            ,typeof(DateTime)),
                new KeyValuePair<string, Type>("blob"            ,typeof(byte[])),
                new KeyValuePair<string, Type>("clob"            ,typeof(byte[])),
                new KeyValuePair<string, Type>("raw"             ,typeof(byte[])),
                new KeyValuePair<string, Type>("oleobject"       ,typeof(byte[])),
                new KeyValuePair<string, Type>("photo"           ,typeof(byte[])),
                new KeyValuePair<string, Type>("picture"         ,typeof(byte[])),
                new KeyValuePair<string, Type>("varchar"         ,typeof(Guid)),
                new KeyValuePair<string, Type>("guid"            ,typeof(Guid))
            });
        }
    }
}
