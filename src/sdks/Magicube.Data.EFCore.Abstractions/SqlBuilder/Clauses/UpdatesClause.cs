using System.Collections.Generic;

namespace Magicube.Data.Abstractions.SqlBuilder.Clauses {
    public class UpdatesClause : Clause{
        public List<UpdateItem> Updates { get; set; }
        public UpdatesClause() {
            Updates = new List<UpdateItem>();
        }
    }

    public class UpdateItem { 
        public string Column { get; set; }
        public object Value  { get; set; }
    }
}
