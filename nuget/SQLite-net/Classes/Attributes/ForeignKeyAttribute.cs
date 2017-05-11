using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite.Attributes {

    /// <summary>
    /// Foreign key
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKeyAttribute : Attribute {

        public Type ReferenceTable { get; private set; }
        public string ReferenceColumn { get; private set; }

        /// <summary>
        /// Set foreign key
        /// </summary>
        /// <param name="referenceTable">The table this foreign key references</param>
        /// <param name="referenceColumn">The name of the column referenced</param>
        public ForeignKeyAttribute(Type referenceTable, string referenceColumn = null) {
            ReferenceTable = referenceTable;
            ReferenceColumn = referenceColumn;
        }

    }
}
