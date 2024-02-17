using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;

namespace Magicube.Data.Abstractions.SqlBuilder {
    public interface ITableProvider { 
        ITableProvider Register(DataTable dt); 
    }

    public class TableProvider : ITableProvider {
        private readonly ConcurrentDictionary<string, DataTable> _dataTables = new ConcurrentDictionary<string, DataTable>();
        public ITableProvider Register(DataTable dt) {
            if (dt == null) throw new ArgumentNullException(nameof(dt));
            _dataTables.AddOrUpdate(dt.TableName, dt, (k, v) => {
                v.Columns.AddRange(dt.Columns.OfType<DataColumn>().ToArray());
                return v;
            });
            return this;
        }
    }
}
