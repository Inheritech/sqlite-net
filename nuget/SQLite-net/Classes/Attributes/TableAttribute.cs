using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite.Attributes {

    /// <summary>
    /// TableAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute {
        /// <summary>
        /// Table name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Define name for Table
        /// </summary>
        /// <param name="name">Name of the table</param>
        public TableAttribute(string name) {
            Name = name;
        }
    }

}
