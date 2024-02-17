namespace Magicube.Data.Abstractions.SqlBuilder.Clauses {
    public class AlterClause : Clause {
        public string     Action { get; }
        public ColumnItem Column { get; }

        public AlterClause(AlterType type, ColumnItem column) {
            Action = type.ToString().ToLower();
            Column = column;
        }
    }

    public enum AlterType {
        Add,
        Drop
    }
}
