using System.Collections.Generic;

namespace Magicube.Data.Abstractions.SqlBuilder.Clauses {
    public class InsertClause : Clause {
        public InsertClause(string[] columns, object[] values) {
            Columns = columns;
            Values  = values;
        }
        public string[] Columns { get; set; }
        public object[] Values  { get; set; }
    }
}
