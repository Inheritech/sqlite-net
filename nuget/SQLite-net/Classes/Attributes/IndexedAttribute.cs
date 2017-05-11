using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite.Attributes {

    /// <summary>
    /// Indexed
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexedAttribute : Attribute {
        /// <summary>
        /// Name of the indexed attribute
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Order of the index
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Is the index unique
        /// </summary>
        public virtual bool Unique { get; set; }

        /// <summary>
        /// Indexed
        /// </summary>
        public IndexedAttribute() {
        }

        /// <summary>
        /// Indexed with options
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="order">Order</param>
        public IndexedAttribute(string name, int order) {
            Name = name;
            Order = order;
        }
    }
}
