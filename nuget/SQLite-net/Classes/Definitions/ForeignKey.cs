using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SQLite.Definitions.TableMapping;

namespace SQLite.Definitions {

    public class ForeignKey {
        public Column KeyColumn { get; private set; }
        public Type ReferencedTable { get; private set; }
        public Column ReferencedColumn { get; private set; }

        public ForeignKey(Column key, Type refTable, Column refCol) {
            KeyColumn = key;
            ReferencedTable = refTable;
            ReferencedColumn = refCol;
        }

    }

}
