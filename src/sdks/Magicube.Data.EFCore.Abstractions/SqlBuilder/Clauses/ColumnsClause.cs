using System;
using System.Collections.Generic;

namespace Magicube.Data.Abstractions.SqlBuilder.Clauses {
    public class ColumnsClause : Clause {
        public ColumnsClause(IEnumerable<ColumnItem> columns) {
            Columns = new List<ColumnItem>(columns);
        }
        public bool             IsDistinct { get; set; }

        public List<ColumnItem> Columns    { get; set; }
    }

    public class ColumnItem  {
        public ColumnItem(string name) {
            Name = name;
        }
        public string Name          { get; set; }
        public Type   Type           { get; set; }
        public int    Size           { get; set; }
        public bool   UniqueKey      { get; set; }
        public bool   PrimaryKey     { get; set; }
        public bool   AutoIncrement  { get; set; }
        public bool   Nullable       { get; set; } = true;
    }
}
