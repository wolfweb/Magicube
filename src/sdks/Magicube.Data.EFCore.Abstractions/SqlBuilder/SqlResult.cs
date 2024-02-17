using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.Data.Abstractions.SqlBuilder {
    public class SqlResult {
        public SqlResult() {
            ParamsBindings = new Dictionary<string, object>();
        }
        public Dictionary<string, object> ParamsBindings { get; set; }

        public string Sql { get; set; } = "";

        public string RawSql() {
            var sql = Sql;
            foreach (var item in ParamsBindings) {
                sql = sql.Replace(item.Key, ChangeToSqlValue(item.Value));
            }
            return sql;
        }

        private string ChangeToSqlValue(object value) {
            if (value == null) {
                return "NULL";
            }

            if (IsArray(value)) {
                var enumerator = (value as IEnumerable).GetEnumerator();
                var c = new List<object>();
                while (enumerator.MoveNext()) {
                    c.Add(ChangeToSqlValue(enumerator.Current));
                }
                return $"({string.Join(",", c)})";
            }

            if (NumberTypes.Contains(value.GetType())) {
                return value.ToString();
            }

            if (value is DateTime date) {
                if (date.Date == date) {
                    return "'" + date.ToString("yyyy-MM-dd") + "'";
                }

                return "'" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }

            if (value is bool vBool) {
                return vBool ? "true" : "false";
            }

            if (value is Enum vEnum) {
                return Convert.ToInt32(vEnum) + $" /* {vEnum} */";
            }

            return "'" + value.ToString() + "'";
        }

        private static bool IsArray(object value) {
            if (value is string) return false;
            if (value is StringValues) return false;
            return value is IEnumerable;
        }

        private static readonly Type[] NumberTypes =
        {
            typeof(int),
            typeof(long),
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(short),
            typeof(ushort),
            typeof(ulong),
        };
    }
}
