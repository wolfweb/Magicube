namespace Magicube.Data.Abstractions.SqlBuilder.Clauses {
    public class OrderbyClause : Clause {
        public OrderbyClause(string column, string orderby) {
            Column = column;
            Orderby = orderby;
        }
        public string Column { get; }
        public string Orderby { get; }
    }
}
