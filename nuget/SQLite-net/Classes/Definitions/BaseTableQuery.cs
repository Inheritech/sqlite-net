using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite.Classes {
    public abstract class BaseTableQuery {
        protected class Ordering {
            public string ColumnName { get; set; }
            public bool Ascending { get; set; }
        }
    }
}
