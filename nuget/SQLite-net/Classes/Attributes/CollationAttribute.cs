using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite.Attributes {

    /// <summary>
    /// Collation
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CollationAttribute : Attribute {

        /// <summary>
        /// Collation value
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="collation">Collation to use</param>
        public CollationAttribute(string collation) {
            Value = collation;
        }
    }
}
